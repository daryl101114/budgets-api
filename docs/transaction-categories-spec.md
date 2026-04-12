# Transaction Categories API Spec

**Base URL:** `http://localhost:5038/api/transactioncategories`
**Auth:** JWT via `app.at` HttpOnly cookie (sent with `credentials: "include"`)

---

## Overview

Transaction categories classify transactions under spending buckets (e.g. "Groceries" under "Lifestyle"). Two kinds exist:

- **System categories** — `userId` is `null`. Read-only. Visible to all users. Cannot be edited or deleted.
- **User categories** — `userId` matches the authenticated user. Full CRUD. Only visible to the owner.

The `GET` endpoints return both system and user-owned categories combined.

---

## Types

```typescript
interface TransactionCategory {
  id: string;            // UUID
  bucketId: string;      // UUID — references a SpendingBucket
  userId: string | null; // null = system category
  categoryName: string;  // max 100 chars
  isFixed: boolean;      // true = fixed expense (rent, insurance), false = variable
  icon: string | null;   // max 50 chars — icon identifier for UI
  color: string | null;  // max 7 chars — hex color e.g. "#FF5733"
  createdAt: string;     // ISO 8601 UTC datetime
}

interface CreateTransactionCategoryRequest {
  bucketId: string;      // required — UUID of the target spending bucket
  categoryName: string;  // required — max 100 chars
  isFixed?: boolean;     // default: false
  icon?: string;         // max 50 chars
  color?: string;        // max 7 chars, hex format
}

interface UpdateTransactionCategoryRequest {
  categoryName?: string; // max 100 chars
  bucketId?: string;     // UUID — move category to a different bucket
  isFixed?: boolean;
  icon?: string;         // max 50 chars
  color?: string;        // max 7 chars, hex format
}
```

All fields in the update request are optional. Only provided fields are applied (partial update via AutoMapper PreCondition).

---

## Endpoints

### GET /api/transactioncategories

Returns all non-deleted categories visible to the authenticated user (system + user-owned).

**Response:** `200 OK`
```json
[
  {
    "id": "a1b2c3d4-...",
    "bucketId": "00000000-0000-0000-0000-000000000004",
    "userId": null,
    "categoryName": "Groceries",
    "isFixed": false,
    "icon": null,
    "color": null,
    "createdAt": "2026-04-11T00:00:00Z"
  }
]
```

Sorted alphabetically by `categoryName`.

---

### GET /api/transactioncategories/{id}

Returns a single category by ID. Only returns it if the category is a system category or belongs to the requesting user.

**Response:** `200 OK` — category object
**Response:** `404 Not Found` — category does not exist or belongs to another user

---

### POST /api/transactioncategories

Creates a new user-owned category. The `userId` is set server-side from the JWT token.

**Request body:** `CreateTransactionCategoryRequest`

**Response:** `201 Created` — returns the created `TransactionCategory` object
**Location header:** points to `GET /api/transactioncategories/{id}`

```json
{
  "id": "newly-generated-uuid",
  "bucketId": "00000000-0000-0000-0000-000000000004",
  "userId": "current-user-uuid",
  "categoryName": "Coffee Shops",
  "isFixed": false,
  "icon": "coffee",
  "color": "#8B4513",
  "createdAt": "2026-04-11T12:00:00Z"
}
```

---

### PUT /api/transactioncategories/{id}

Partial update on a user-owned category. System categories cannot be modified.

**Request body:** `UpdateTransactionCategoryRequest` — only include the fields you want to change.

**Response:** `204 No Content` — success
**Response:** `404 Not Found` — category does not exist
**Response:** `403 Forbidden` — system category or belongs to another user

---

### DELETE /api/transactioncategories/{id}

Soft-deletes a user-owned category. System categories cannot be deleted.

**Response:** `204 No Content` — success
**Response:** `404 Not Found` — category does not exist
**Response:** `403 Forbidden` — system category or belongs to another user

---

## Spending Buckets (reference)

Categories must reference a valid spending bucket. These are the seeded bucket IDs:

| Bucket Name           | ID                                     |
|-----------------------|----------------------------------------|
| Other                 | `00000000-0000-0000-0000-000000000001` |
| Housing               | `00000000-0000-0000-0000-000000000002` |
| Transportation        | `00000000-0000-0000-0000-000000000003` |
| Lifestyle             | `00000000-0000-0000-0000-000000000004` |
| Debt                  | `00000000-0000-0000-0000-000000000005` |
| Savings & Investments | `00000000-0000-0000-0000-000000000006` |
| Income                | `00000000-0000-0000-0000-000000000007` |

---

## Default System Categories

These are pre-seeded with `userId = null`. They appear for every user and cannot be edited or deleted.

| Category Name       | Bucket              | IsFixed |
|---------------------|---------------------|---------|
| Rent / Mortgage     | Housing             | true    |
| Utilities           | Housing             | true    |
| Home Insurance      | Housing             | true    |
| Car Payment         | Transportation      | true    |
| Gas                 | Transportation      | false   |
| Auto Insurance      | Transportation      | true    |
| Groceries           | Lifestyle           | false   |
| Dining Out          | Lifestyle           | false   |
| Entertainment       | Lifestyle           | false   |
| Streaming Services  | Lifestyle           | true    |
| Gym & Fitness       | Lifestyle           | true    |
| Personal Care       | Lifestyle           | false   |
| Credit Card Payment | Debt                | false   |
| Student Loan        | Debt                | true    |
| Savings Transfer    | Savings & Investments | false |
| Investment Deposit  | Savings & Investments | false |
| Salary              | Income              | true    |
| Freelance           | Income              | false   |
| Remittance          | Other               | false   |
| Peer Payment        | Other               | false   |

---

## Error Responses

All error responses return a plain string body.

| Status | Meaning                                            |
|--------|----------------------------------------------------|
| 401    | Missing or invalid JWT                             |
| 403    | Attempting to modify/delete a system or non-owned category |
| 404    | Category not found or soft-deleted                 |
| 500    | Unexpected server error                            |

---

## Usage Notes

- Use `isFixed` to visually distinguish fixed monthly expenses from variable ones in the UI.
- `icon` and `color` are free-form strings for frontend rendering. The backend does not validate icon names or hex format beyond max length.
- When a category is deleted, the server soft-deletes it (`deletedAt` is set). Transactions referencing that category retain their `subcategoryId` — they are not orphaned.
- The `GET` list endpoint does not support pagination. The expected category count per user is small (20 system + a handful of user-created).

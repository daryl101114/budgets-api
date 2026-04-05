/**
 * Budgify API Schema
 * Auto-generated from backend DTOs — April 2026
 *
 * Base URL (dev):  http://localhost:5038
 * Auth:           JWT stored in app.at cookie (sent automatically with credentials: "include")
 */

// ─────────────────────────────────────────────────────────────────────────────
// ENUMS
// ─────────────────────────────────────────────────────────────────────────────

export type WalletType =
  | "Checking"
  | "Savings"
  | "CreditCard"
  | "Cash"
  | "Loan"
  | "Investment";

export type Currency =
  | "USD"
  | "EUR"
  | "GBP"
  | "CAD"
  | "AUD"
  | "JPY"
  | "CHF"
  | "CNY"
  | "INR"
  | "MXN";

export type TransactionType = "Income" | "Expense";

export type TransactionSource = "Manual" | "Import";

export type ImportBatchStatus = "Reviewing" | "Confirmed" | "RolledBack";

export type ImportStatus = "Pending" | "Confirmed" | "Rejected";

// ─────────────────────────────────────────────────────────────────────────────
// MODELS
// ─────────────────────────────────────────────────────────────────────────────

export interface User {
  firstName: string;
  lastName: string;
  email: string;
  currency: string;
  createdAt: string; // ISO datetime
  updatedAt: string | null;
}

export interface UserWithWallets {
  id: string; // UUID
  firstName: string;
  lastName: string;
  email: string;
  currency: string;
  wallets: Wallet[];
}

export interface Wallet {
  id: string;
  walletName: string;
  walletType: WalletType;
  institution: string | null;
  balance: number;
  currency: Currency;
  lastSynced: string | null; // ISO datetime
  createdAt: string;
  updatedAt: string;
}

export interface Transaction {
  id: string;
  walletId: string;
  userId: string;
  subcategoryId: string | null;
  importId: string | null;
  amount: number;
  transactionType: TransactionType;
  date: string; // "YYYY-MM-DD"
  description: string | null;
  rawDescription: string | null;
  bankTxnType: string | null;
  balanceAfter: number | null;
  checkNumber: string | null;
  isRecurring: boolean;
  duplicateHash: string | null;
  source: TransactionSource;
  createdAt: string;
  updatedAt: string;
}

export interface TransactionCategory {
  id: string;
  bucketId: string;
  userId: string | null; // null = system category
  categoryName: string;
  isFixed: boolean;
  icon: string | null;
  color: string | null; // hex e.g. "#FF5733"
  createdAt: string;
}

export interface SpendingBucket {
  id: string;
  bucketName: string;
}

export interface Budget {
  id: string;
  name: string;
  totalIncome: number;
  startDate: string; // "YYYY-MM-DD"
  endDate: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  allocations: BudgetAllocation[];
}

export interface BudgetAllocation {
  id: string;
  bucketId: string;
  bucketName: string;
  percentage: number;
  allocatedAmount: number;
}

export interface ImportBatch {
  id: string;
  walletId: string;
  fileName: string;
  status: ImportBatchStatus;
  totalRows: number;
  importedCount: number;
  duplicateCount: number;
  skippedCount: number;
  createdAt: string;
  confirmedAt: string | null;
  rolledBackAt: string | null;
}

export interface ImportBatchDetail extends ImportBatch {
  pendingDuplicates: number;
  readyToConfirm: boolean;
  transactions: ImportTransaction[];
}

export interface ImportTransaction {
  id: string;
  amount: number;
  transactionType: TransactionType;
  date: string; // "YYYY-MM-DD"
  description: string | null;
  rawDescription: string | null;
  bankTxnType: string | null;
  isDuplicate: boolean;
  importStatus: ImportStatus;
}

// ─────────────────────────────────────────────────────────────────────────────
// REQUEST BODIES
// ─────────────────────────────────────────────────────────────────────────────

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  currency?: string; // default: "USD"
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface CreateWalletRequest {
  walletName: string;
  walletType: WalletType;
  institution?: string;
  balance?: number; // default: 0
  currency: Currency;
}

export interface UpdateWalletRequest {
  walletName?: string;
  institution?: string;
  currency?: Currency;
}

export interface CreateTransactionRequest {
  walletId: string;
  subcategoryId?: string;
  amount: number; // must be > 0
  transactionType: TransactionType;
  date: string; // "YYYY-MM-DD"
  description?: string;
  isRecurring?: boolean; // default: false
}

export interface UpdateTransactionRequest {
  description?: string;
  date?: string; // "YYYY-MM-DD"
  amount?: number;
  transactionType?: TransactionType;
  subcategoryId?: string;
  isRecurring?: boolean;
}

export interface CreateTransactionCategoryRequest {
  bucketId: string;
  categoryName: string;
  isFixed?: boolean; // default: false
  icon?: string;
  color?: string; // hex
}

export interface UpdateTransactionCategoryRequest {
  categoryName?: string;
  bucketId?: string;
  isFixed?: boolean;
  icon?: string;
  color?: string;
}

export interface CreateBudgetRequest {
  name: string;
  totalIncome: number; // must be > 0
  startDate: string; // "YYYY-MM-DD"
  endDate: string;
  isActive?: boolean; // default: false
}

export interface UpdateBudgetRequest {
  name?: string;
  totalIncome?: number;
  startDate?: string;
  endDate?: string;
}

export interface CreateBudgetAllocationRequest {
  bucketId: string;
  percentage: number; // 0.01–100.00
}

export interface UpdateBudgetAllocationRequest {
  bucketId?: string;
  percentage?: number;
}

export interface BulkReviewRequest {
  transactionIds: string[];
}

// ─────────────────────────────────────────────────────────────────────────────
// API ENDPOINTS
// ─────────────────────────────────────────────────────────────────────────────

export const API_BASE = "http://localhost:5038/api";

export const ENDPOINTS = {
  // Auth
  register:     `${API_BASE}/authentication/register`,
  login:        `${API_BASE}/authentication/authenticate`,

  // Wallets
  wallets:      `${API_BASE}/wallets`,
  wallet:       (id: string) => `${API_BASE}/wallets/${id}`,

  // Transactions
  transactions: `${API_BASE}/transactions`,
  transaction:  (id: string) => `${API_BASE}/transactions/${id}`,

  // Transaction Categories
  categories:   `${API_BASE}/transactioncategories`,
  category:     (id: string) => `${API_BASE}/transactioncategories/${id}`,

  // Budgets
  budgets:      `${API_BASE}/budgets`,
  budget:       (id: string) => `${API_BASE}/budgets/${id}`,

  // Budget Allocations
  allocations:  (budgetId: string) => `${API_BASE}/budgets/${budgetId}/allocations`,
  allocation:   (budgetId: string, id: string) => `${API_BASE}/budgets/${budgetId}/allocations/${id}`,

  // Import
  importUpload: (walletId: string) => `${API_BASE}/import/wallets/${walletId}`,
  imports:      `${API_BASE}/import`,
  importDetail: (id: string) => `${API_BASE}/import/${id}`,
  importApprove:(id: string) => `${API_BASE}/import/${id}/transactions/approve`,
  importReject: (id: string) => `${API_BASE}/import/${id}/transactions/reject`,
  importConfirm:(id: string) => `${API_BASE}/import/${id}/confirm`,
  importRollback:(id: string) => `${API_BASE}/import/${id}/rollback`,
} as const;

// ─────────────────────────────────────────────────────────────────────────────
// BASE FETCH HELPER
// Automatically sends cookies (app.at) and sets Content-Type JSON.
// ─────────────────────────────────────────────────────────────────────────────

async function request<T>(
  url: string,
  options: RequestInit = {}
): Promise<T> {
  const res = await fetch(url, {
    ...options,
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      ...options.headers,
    },
  });

  if (!res.ok) {
    const error = await res.text().catch(() => res.statusText);
    throw new Error(`${res.status}: ${error}`);
  }

  // 204 No Content
  if (res.status === 204) return undefined as T;

  return res.json();
}

// ─────────────────────────────────────────────────────────────────────────────
// API CLIENT
// ─────────────────────────────────────────────────────────────────────────────

export const api = {
  // ── Auth ──────────────────────────────────────────────────────────────────
  auth: {
    register: (body: RegisterRequest) =>
      request<User>(ENDPOINTS.register, { method: "POST", body: JSON.stringify(body) }),

    login: (body: LoginRequest) =>
      request<User>(ENDPOINTS.login, { method: "POST", body: JSON.stringify(body) }),
  },

  // ── Wallets ───────────────────────────────────────────────────────────────
  wallets: {
    getAll: () =>
      request<UserWithWallets>(ENDPOINTS.wallets),

    getOne: (id: string) =>
      request<Wallet>(ENDPOINTS.wallet(id)),

    create: (body: CreateWalletRequest) =>
      request<Wallet>(ENDPOINTS.wallets, { method: "POST", body: JSON.stringify(body) }),

    update: (id: string, body: UpdateWalletRequest) =>
      request<Wallet>(ENDPOINTS.wallet(id), { method: "PUT", body: JSON.stringify(body) }),

    delete: (id: string) =>
      request<void>(ENDPOINTS.wallet(id), { method: "DELETE" }),
  },

  // ── Transactions ──────────────────────────────────────────────────────────
  transactions: {
    getAll: (walletId: string) =>
      request<Transaction[]>(`${ENDPOINTS.transactions}?walletId=${walletId}`),

    getOne: (id: string) =>
      request<Transaction>(ENDPOINTS.transaction(id)),

    create: (body: CreateTransactionRequest) =>
      request<Transaction>(ENDPOINTS.transactions, { method: "POST", body: JSON.stringify(body) }),

    update: (id: string, body: UpdateTransactionRequest) =>
      request<Transaction>(ENDPOINTS.transaction(id), { method: "PUT", body: JSON.stringify(body) }),

    delete: (id: string) =>
      request<void>(ENDPOINTS.transaction(id), { method: "DELETE" }),
  },

  // ── Transaction Categories ────────────────────────────────────────────────
  categories: {
    getAll: () =>
      request<TransactionCategory[]>(ENDPOINTS.categories),

    getOne: (id: string) =>
      request<TransactionCategory>(ENDPOINTS.category(id)),

    create: (body: CreateTransactionCategoryRequest) =>
      request<TransactionCategory>(ENDPOINTS.categories, { method: "POST", body: JSON.stringify(body) }),

    update: (id: string, body: UpdateTransactionCategoryRequest) =>
      request<TransactionCategory>(ENDPOINTS.category(id), { method: "PUT", body: JSON.stringify(body) }),

    delete: (id: string) =>
      request<void>(ENDPOINTS.category(id), { method: "DELETE" }),
  },

  // ── Budgets ───────────────────────────────────────────────────────────────
  budgets: {
    getAll: () =>
      request<Budget[]>(ENDPOINTS.budgets),

    getOne: (id: string) =>
      request<Budget>(ENDPOINTS.budget(id)),

    create: (body: CreateBudgetRequest) =>
      request<Budget>(ENDPOINTS.budgets, { method: "POST", body: JSON.stringify(body) }),

    update: (id: string, body: UpdateBudgetRequest) =>
      request<Budget>(ENDPOINTS.budget(id), { method: "PUT", body: JSON.stringify(body) }),

    delete: (id: string) =>
      request<void>(ENDPOINTS.budget(id), { method: "DELETE" }),
  },

  // ── Budget Allocations ────────────────────────────────────────────────────
  allocations: {
    getAll: (budgetId: string) =>
      request<BudgetAllocation[]>(ENDPOINTS.allocations(budgetId)),

    getOne: (budgetId: string, id: string) =>
      request<BudgetAllocation>(ENDPOINTS.allocation(budgetId, id)),

    create: (budgetId: string, body: CreateBudgetAllocationRequest) =>
      request<BudgetAllocation>(ENDPOINTS.allocations(budgetId), { method: "POST", body: JSON.stringify(body) }),

    update: (budgetId: string, id: string, body: UpdateBudgetAllocationRequest) =>
      request<BudgetAllocation>(ENDPOINTS.allocation(budgetId, id), { method: "PUT", body: JSON.stringify(body) }),

    delete: (budgetId: string, id: string) =>
      request<void>(ENDPOINTS.allocation(budgetId, id), { method: "DELETE" }),
  },

  // ── Import ────────────────────────────────────────────────────────────────
  import: {
    upload: (walletId: string, file: File) => {
      const form = new FormData();
      form.append("file", file);
      return request<ImportBatch>(ENDPOINTS.importUpload(walletId), {
        method: "POST",
        body: form,
        headers: {}, // let browser set multipart boundary
      });
    },

    getAll: () =>
      request<ImportBatch[]>(ENDPOINTS.imports),

    getOne: (id: string) =>
      request<ImportBatchDetail>(ENDPOINTS.importDetail(id)),

    approve: (id: string, body: BulkReviewRequest) =>
      request<void>(ENDPOINTS.importApprove(id), { method: "PATCH", body: JSON.stringify(body) }),

    reject: (id: string, body: BulkReviewRequest) =>
      request<void>(ENDPOINTS.importReject(id), { method: "PATCH", body: JSON.stringify(body) }),

    confirm: (id: string) =>
      request<ImportBatch>(ENDPOINTS.importConfirm(id), { method: "POST" }),

    rollback: (id: string) =>
      request<ImportBatch>(ENDPOINTS.importRollback(id), { method: "POST" }),
  },
};

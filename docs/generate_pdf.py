"""
Generates docs/Budgify-v2.1.pdf using fpdf2.
Run from anywhere: python docs/generate_pdf.py
"""
from fpdf import FPDF
import os

OUT = os.path.join(os.path.dirname(__file__), "Budgify-v2.1.pdf")

# -- Colour palette ----------------------------------------------------------
NAVY   = (10,  22,  40)   # headings / header bg
BLUE   = (37,  99, 235)   # h2 underline / accents
LGREY  = (249, 250, 251)  # even-row table bg
MGREY  = (229, 231, 235)  # table borders
DGREY  = (55,  65,  81)   # body text
YELLOW = (254, 252, 232)  # "changed" callout bg
GREEN  = (220, 252, 231)  # "added" callout bg
PURPLE = (237, 233, 254)  # "planned" callout bg
YGOLD  = (234, 179,   8)  # yellow border
GGREEN = ( 22, 163,  74)  # green border
PPURP  = ( 99, 102, 241)  # purple border

class PDF(FPDF):
    def __init__(self):
        super().__init__(orientation="P", unit="mm", format="A4")
        self.set_auto_page_break(auto=True, margin=20)
        self.set_margins(20, 20, 20)
        self._toc = []          # (title, level, page)
        self._after_cover = False

    # -- Header / footer --------------------------------------------------
    def header(self):
        if not self._after_cover:
            return
        self.set_font("Helvetica", "I", 8)
        self.set_text_color(*DGREY)
        self.cell(0, 8, "Budgify Development Documentation  .  v2.1  .  April 2026", align="C")
        self.ln(2)

    def footer(self):
        if not self._after_cover:
            return
        self.set_y(-14)
        self.set_font("Helvetica", "I", 8)
        self.set_text_color(*DGREY)
        self.cell(0, 8, f"Page {self.page_no()}", align="C")

    # -- Typography helpers ------------------------------------------------
    def h1(self, text):
        self.set_font("Helvetica", "B", 26)
        self.set_text_color(*NAVY)
        self.multi_cell(0, 10, text)
        self.ln(1)

    def h2(self, text):
        self.ln(4)
        self.set_font("Helvetica", "B", 15)
        self.set_text_color(*NAVY)
        self.multi_cell(0, 8, text)
        # blue underline
        self.set_draw_color(*BLUE)
        self.set_line_width(0.7)
        y = self.get_y()
        self.line(self.l_margin, y, self.w - self.r_margin, y)
        self.set_line_width(0.2)
        self.set_draw_color(*MGREY)
        self.ln(4)
        self._toc.append((text, 2, self.page_no()))

    def h3(self, text):
        self.ln(3)
        self.set_font("Helvetica", "B", 11)
        self.set_text_color(*NAVY)
        self.multi_cell(0, 7, text)
        self.ln(1)

    def body(self, text, indent=0):
        self.set_font("Helvetica", "", 9.5)
        self.set_text_color(*DGREY)
        self.set_x(self.l_margin + indent)
        self.multi_cell(self.w - self.l_margin - self.r_margin - indent, 5.5, text)
        self.ln(1)

    def bullet(self, text, indent=4):
        self.set_font("Helvetica", "", 9.5)
        self.set_text_color(*DGREY)
        x = self.l_margin + indent
        self.set_x(x)
        self.cell(4, 5.5, "-")
        self.set_x(x + 4)
        self.multi_cell(self.w - self.l_margin - self.r_margin - indent - 4, 5.5, text)

    def numbered(self, n, text, indent=4):
        self.set_font("Helvetica", "", 9.5)
        self.set_text_color(*DGREY)
        x = self.l_margin + indent
        self.set_x(x)
        self.cell(6, 5.5, f"{n}.")
        self.set_x(x + 6)
        self.multi_cell(self.w - self.l_margin - self.r_margin - indent - 6, 5.5, text)

    def callout(self, text, style="changed"):
        colours = {
            "changed": (YELLOW, YGOLD),
            "added":   (GREEN,  GGREEN),
            "planned": (PURPLE, PPURP),
        }
        bg, border = colours.get(style, (LGREY, MGREY))
        self.set_fill_color(*bg)
        self.set_draw_color(*border)
        self.set_line_width(0.5)
        x, y = self.get_x(), self.get_y()
        w = self.w - self.l_margin - self.r_margin
        # estimate height
        lines = self.multi_cell(w - 6, 5, text, dry_run=True, output="LINES")
        h = len(lines) * 5 + 6
        self.rect(x, y, 2, h, style="F")          # left bar
        self.rect(x + 2, y, w - 2, h, style="F")  # background
        self.set_text_color(*DGREY)
        self.set_font("Helvetica", "", 9)
        self.set_xy(x + 5, y + 3)
        self.multi_cell(w - 8, 5, text)
        self.set_y(y + h + 2)
        self.set_line_width(0.2)
        self.set_draw_color(*MGREY)

    def code(self, text):
        self.set_font("Courier", "", 8.5)
        self.set_fill_color(15, 23, 42)
        self.set_text_color(226, 232, 240)
        self.set_x(self.l_margin)
        w = self.w - self.l_margin - self.r_margin
        lines = text.split("\n")
        h = len(lines) * 5 + 6
        self.rect(self.l_margin, self.get_y(), w, h, style="F")
        self.set_xy(self.l_margin + 4, self.get_y() + 3)
        for line in lines:
            self.set_x(self.l_margin + 4)
            self.cell(w - 8, 5, line)
            self.ln(5)
        self.set_text_color(*DGREY)
        self.set_font("Helvetica", "", 9.5)
        self.ln(2)

    # -- Table -------------------------------------------------------------
    def table(self, headers, rows, col_widths=None):
        w = self.w - self.l_margin - self.r_margin
        if col_widths is None:
            col_widths = [w / len(headers)] * len(headers)
        else:
            # scale to page width
            total = sum(col_widths)
            col_widths = [c / total * w for c in col_widths]

        row_h = 6

        def draw_row(cells, is_header=False, shade=False, strikethrough=False):
            x0, y0 = self.l_margin, self.get_y()
            # measure height
            max_lines = 1
            for i, cell in enumerate(cells):
                if is_header:
                    self.set_font("Helvetica", "B", 8.5)
                else:
                    self.set_font("Helvetica", "", 8.5)
                lines = self.multi_cell(col_widths[i] - 3, row_h, str(cell),
                                        dry_run=True, output="LINES")
                max_lines = max(max_lines, len(lines))
            cell_h = max_lines * row_h + 2

            # check page break
            if self.get_y() + cell_h > self.h - self.b_margin:
                self.add_page()
                x0, y0 = self.l_margin, self.get_y()

            # fill
            if is_header:
                self.set_fill_color(*NAVY)
                self.set_text_color(255, 255, 255)
            elif shade:
                self.set_fill_color(*LGREY)
                self.set_text_color(*DGREY)
            else:
                self.set_fill_color(255, 255, 255)
                self.set_text_color(*DGREY)

            if strikethrough:
                self.set_text_color(180, 180, 180)

            for i, cell in enumerate(cells):
                self.set_xy(x0, y0)
                self.rect(x0, y0, col_widths[i], cell_h, style="FD")
                self.set_xy(x0 + 1.5, y0 + 1)
                if is_header:
                    self.set_font("Helvetica", "B", 8.5)
                else:
                    self.set_font("Helvetica", "", 8.5)
                self.multi_cell(col_widths[i] - 3, row_h, str(cell))
                x0 += col_widths[i]
            self.set_xy(self.l_margin, y0 + cell_h)

        draw_row(headers, is_header=True)
        for i, row in enumerate(rows):
            if isinstance(row, dict) and row.get("_strike"):
                draw_row(row["cells"], shade=(i % 2 == 0), strikethrough=True)
            else:
                draw_row(row, shade=(i % 2 == 0))
        self.ln(3)
        self.set_draw_color(*MGREY)

    def pill(self, text, style="done"):
        colours = {
            "done":    ((209, 250, 229), (6, 95, 70)),
            "planned": ((237, 233, 254), (76, 29, 149)),
            "future":  ((243, 244, 246), (107, 114, 128)),
        }
        bg, fg = colours.get(style, ((243,244,246),(107,114,128)))
        self.set_font("Helvetica", "B", 7.5)
        w = self.get_string_width(text) + 6
        self.set_fill_color(*bg)
        self.set_text_color(*fg)
        self.cell(w, 5, text, fill=True)
        self.set_font("Helvetica", "", 9.5)
        self.set_text_color(*DGREY)


# ============================================================================
pdf = PDF()
pdf.add_page()

# -- COVER --------------------------------------------------------------------
pdf.ln(30)
pdf.set_font("Helvetica", "B", 48)
pdf.set_text_color(*NAVY)
pdf.cell(0, 18, "Budgify", align="C")
pdf.ln(18)

pdf.set_font("Helvetica", "", 14)
pdf.set_text_color(107, 114, 128)
pdf.cell(0, 8, "Development Documentation", align="C")
pdf.ln(14)

# Blue divider
pdf.set_fill_color(*BLUE)
mid = pdf.w / 2
pdf.rect(mid - 20, pdf.get_y(), 40, 3, style="F")
pdf.ln(12)

# Meta box
pdf.set_fill_color(240, 245, 255)
bx, by = 30, pdf.get_y()
bw = pdf.w - 60
bh = 72
pdf.rect(bx, by, bw, bh, style="F")
meta = [
    ("Version", "2.1 -- Implementation Aligned"),
    ("Date", "April 2026"),
    ("Framework", "ASP.NET Core 8 Web API"),
    ("Database", "SQL Server (SQLEXPRESS) via EF Core 8"),
    ("Auth", "JWT Bearer (HttpOnly app.at cookie)"),
    ("Libraries", "AutoMapper  \u00b7  CsvHelper  \u00b7  Serilog"),
]
pdf.set_xy(bx + 10, by + 8)
for label, val in meta:
    pdf.set_font("Helvetica", "B", 9)
    pdf.set_text_color(*NAVY)
    pdf.set_x(bx + 10)
    pdf.cell(40, 6, label + ":")
    pdf.set_font("Helvetica", "", 9)
    pdf.set_text_color(*DGREY)
    pdf.cell(0, 6, val)
    pdf.ln(9)

pdf.ln(bh - (pdf.get_y() - by) + 10)
pdf.set_font("Helvetica", "", 10)
pdf.set_text_color(156, 163, 175)
pdf.cell(0, 8, "Personal Finance Management Application", align="C")
pdf.ln(6)
pdf.cell(0, 8, "Includes: Data Model  \u00b7  CSV Import Architecture  \u00b7  API Specification  \u00b7  Implementation Roadmap", align="C")

# Switch on header/footer for subsequent pages
pdf._after_cover = True

# -- SECTION 1 ----------------------------------------------------------------
pdf.add_page()
pdf.h2("1. Project Overview")

pdf.h3("1.1 Purpose")
pdf.body("Budgify is a personal finance management application that enables users to track income and expenses across multiple wallets, plan budgets using percentage-based allocation, import bank statement CSVs, and gain insights into their spending behavior. This document serves as the single source of truth for the application's data architecture, feature specifications, and implementation plan.")

pdf.h3("1.2 Core Capabilities")
caps = [
    "Multi-wallet management (checking, savings, credit card, cash accounts)",
    "Manual transaction entry and CSV bank statement imports",
    "Percentage-based budget planning with spending buckets",
    "Smart transaction categorization with learning capabilities  [Planned]",
    "Fixed vs variable expense tracking",
    "Overspend detection and budget monitoring  [Planned]",
    "Import batch tracking with batch approve/reject and rollback support",
]
for c in caps:
    pdf.bullet(c)
pdf.ln(2)

pdf.h3("1.3 Technical Stack")
pdf.callout("The original document was stack-agnostic (MySQL recommended). The implemented stack is ASP.NET Core 8 + SQL Server.", "changed")
pdf.table(
    ["Layer", "Technology"],
    [
        ["Framework", "ASP.NET Core 8 Web API"],
        ["ORM", "Entity Framework Core 8 (Code-First, Migrations)"],
        ["Database", "SQL Server (SQLEXPRESS)"],
        ["Auth", "JWT Bearer -- token as app.at HttpOnly cookie, sent as Authorization: Bearer"],
        ["Object Mapping", "AutoMapper"],
        ["Logging", "Serilog"],
        ["CSV Parsing", "CsvHelper 33.1.0"],
        ["Password Hashing", "ASP.NET Identity PasswordHasher<User>"],
    ],
    col_widths=[1, 3],
)
pdf.body("All IDs are GUIDs (v4). All timestamps are UTC.")
pdf.callout("URL versioning removed: The original spec used /api/v1/ throughout. The implemented API uses unversioned routes: /api/", "changed")

# -- SECTION 2 ----------------------------------------------------------------
pdf.add_page()
pdf.h2("2. Data Model")
pdf.body("All entities include soft-delete support via a DeletedAt nullable timestamp, except SpendingBucket and BudgetAllocation which are managed without soft-delete in the current implementation.")

# 2.1
pdf.h3("2.1 User  [Done]")
pdf.callout("The original spec had a single name field. Implementation splits this into FirstName and LastName. Settings is stored as a JSON string (not JSONB). Email is used for login -- there is no separate username field.", "changed")
pdf.table(
    ["Field", "Type", "Required", "Description"],
    [
        ["Id", "Guid", "PK", "Auto-generated unique identifier."],
        ["FirstName", "string(50)", "Yes", "First name.  (was: single 'name' field)"],
        ["LastName", "string(50)", "Yes", "Last name.  (was: single 'name' field)"],
        ["Email", "string(255)", "Yes", "Login email. Must be unique."],
        ["PasswordHash", "string", "Yes", "ASP.NET Identity PasswordHasher. Never plaintext."],
        ["Currency", "string", "Yes", "Preferred currency code. Default: USD."],
        ["Settings", "string?", "No", "JSON string for user preferences."],
        ["CreatedAt", "DateTime", "Auto", "UTC."],
        ["UpdatedAt", "DateTime?", "Auto", "UTC."],
        ["DeletedAt", "DateTime?", "No", "Soft-delete timestamp. Null if active."],
    ],
    col_widths=[1.2, 1, 0.8, 2.5],
)

# 2.2
pdf.h3("2.2 Wallet  [Done]")
pdf.table(
    ["Field", "Type", "Required", "Description"],
    [
        ["Id", "Guid", "PK", "Auto-generated."],
        ["UserId", "Guid (FK)", "Yes", "References User. Owner of this wallet."],
        ["WalletName", "string(100)", "Yes", "User-defined name (e.g., 'Chase Checking')."],
        ["WalletType", "WalletType (enum)", "Yes", "Stored as string. See Appendix."],
        ["Institution", "string(100)?", "No", "Bank name. Aids CSV format detection."],
        ["Balance", "decimal(12,2)", "Yes", "Current balance. Updated via delta on every transaction."],
        ["Currency", "Currency (enum)", "Yes", "Stored as string. Inherits from User by default."],
        ["LastSynced", "DateTime?", "No", "Last CSV import timestamp."],
        ["CreatedAt", "DateTime", "Auto", "UTC."],
        ["UpdatedAt", "DateTime", "Auto", "UTC."],
        ["DeletedAt", "DateTime?", "No", "Soft-delete timestamp."],
    ],
    col_widths=[1.2, 1.3, 0.8, 2.2],
)
pdf.body("Note: Balance is maintained via delta adjustments -- not recalculated from all transactions. Rollback reverses only the delta of confirmed transactions.")

# 2.3
pdf.add_page()
pdf.h3("2.3 Transaction  [Done]")
pdf.callout("New in Phase 2.8: IsDuplicate (bool) and ImportStatus (nullable enum) added to support the import review workflow.", "added")
pdf.table(
    ["Field", "Type", "Required", "Description"],
    [
        ["Id", "Guid", "PK", "Auto-generated."],
        ["WalletId", "Guid (FK)", "Yes", "References Wallet."],
        ["UserId", "Guid (FK)", "Yes", "Denormalized for fast permission checks."],
        ["SubcategoryId", "Guid? (FK)", "No", "References TransactionCategory. Null = uncategorized."],
        ["ImportId", "Guid?", "No", "References ImportBatch. Null for manual transactions."],
        ["Amount", "decimal(12,2)", "Yes", "Always positive. Direction set by TransactionType."],
        ["TransactionType", "Enum", "Yes", "Income or Expense. Stored as string."],
        ["Date", "DateOnly", "Yes", "Posting date for CSV imports."],
        ["Description", "string(500)?", "No", "User-facing (cleaned) description."],
        ["RawDescription", "string(1000)?", "No", "Original bank description, preserved as-is."],
        ["BankTxnType", "string(50)?", "No", "Bank type code (e.g., DEBIT_CARD, ACH_CREDIT)."],
        ["BalanceAfter", "decimal(12,2)?", "No", "Running balance from bank statement. Reconciliation only."],
        ["CheckNumber", "string(20)?", "No", "Check or slip number if applicable."],
        ["IsRecurring", "bool", "Yes", "Default: false."],
        ["RecurringRuleId", "Guid?", "No", "Reserved for future RecurringRule feature."],
        ["DuplicateHash", "string(64)?", "No", "SHA-256 hex string. See Section 4.5."],
        ["Source", "TransactionSource", "Yes", "Manual or Import. Stored as string. Default: Manual."],
        ["IsDuplicate  [NEW]", "bool", "Yes", "Set true during import when hash matches existing transaction."],
        ["ImportStatus  [NEW]", "ImportStatus?", "No", "Null for manual. Pending/Confirmed/Rejected for imports."],
        ["CreatedAt", "DateTime", "Auto", "UTC."],
        ["UpdatedAt", "DateTime", "Auto", "UTC."],
        ["DeletedAt", "DateTime?", "No", "Soft-delete timestamp."],
    ],
    col_widths=[1.5, 1.2, 0.8, 2],
)
pdf.body("Indexes: (UserId, Date) . (WalletId, Date) . DuplicateHash")

# 2.4
pdf.h3("2.4 TransactionCategory  [Done]")
pdf.callout("New fields added: Icon (string) and Color (hex string) for UI rendering -- not in original spec.", "added")
pdf.table(
    ["Field", "Type", "Required", "Description"],
    [
        ["Id", "Guid", "PK", "Auto-generated."],
        ["BucketId", "Guid (FK)", "Yes", "References SpendingBucket."],
        ["UserId", "Guid? (FK)", "No", "Null = system category. Set = user-created."],
        ["CategoryName", "string(100)", "Yes", "Display name (e.g., 'Groceries', 'Netflix')."],
        ["IsFixed", "bool", "Yes", "True for fixed expenses. Default: false."],
        ["Icon  [NEW]", "string(50)?", "No", "Icon identifier for UI rendering."],
        ["Color  [NEW]", "string(7)?", "No", "Hex color code, e.g., #FF5733."],
        ["CreatedAt", "DateTime", "Auto", "UTC."],
        ["DeletedAt", "DateTime?", "No", "Soft-delete. System categories cannot be deleted."],
    ],
    col_widths=[1.3, 1.1, 0.8, 2.3],
)

# 2.5
pdf.h3("2.5 SpendingBucket  [Done -- Simplified]")
pdf.callout("Simplified: Original spec included user_id, is_custom, icon, sort_order, deleted_at. The current implementation only stores Id and BucketName. All buckets are system-managed (seeded). User-created custom buckets are planned for Phase 5.", "changed")
pdf.table(
    ["Field", "Type", "Required", "Description"],
    [
        ["Id", "Guid", "PK", "Unique identifier."],
        ["BucketName", "string(100)", "Yes", "Display name (e.g., 'Housing', 'Lifestyle')."],
        ["user_id  [REMOVED]", "Guid?", "--", "Removed. All buckets are system-managed."],
        ["is_custom  [REMOVED]", "bool", "--", "Removed."],
        ["icon  [REMOVED]", "string", "--", "Removed."],
        ["sort_order  [REMOVED]", "int", "--", "Removed."],
        ["deleted_at  [REMOVED]", "DateTime?", "--", "Removed. Buckets are not soft-deleted."],
    ],
    col_widths=[1.5, 1, 0.8, 2.2],
)

# 2.6
pdf.add_page()
pdf.h3("2.6 Budget  [Done]")
pdf.table(
    ["Field", "Type", "Required", "Description"],
    [
        ["Id", "Guid", "PK", "Auto-generated."],
        ["UserId", "Guid (FK)", "Yes", "References User. Owner of this budget."],
        ["Name", "string(100)", "Yes", "User-defined label (e.g., 'March 2026')."],
        ["TotalIncome", "decimal(12,2)", "Yes", "Total income being budgeted."],
        ["StartDate", "DateOnly", "Yes", "First day of budget period."],
        ["EndDate", "DateOnly", "Yes", "Last day of budget period."],
        ["IsActive", "bool", "Yes", "Only one active budget per user. Default: false."],
        ["CreatedAt", "DateTime", "Auto", "UTC."],
        ["UpdatedAt", "DateTime", "Auto", "UTC."],
        ["DeletedAt", "DateTime?", "No", "Soft-delete timestamp."],
    ],
    col_widths=[1.2, 1.1, 0.8, 2.4],
)
pdf.body("Constraint: Partial unique index on (UserId) where IsActive = 1. Enforced at the database level by EF Core migration.")

# 2.7
pdf.h3("2.7 BudgetAllocation  [Done]")
pdf.callout("Constraint relaxed: Original spec required allocations to sum to exactly 100%. The implementation validates that total does not exceed 100% -- it is not required to equal exactly 100%.", "changed")
pdf.table(
    ["Field", "Type", "Required", "Description"],
    [
        ["Id", "Guid", "PK", "Auto-generated."],
        ["BudgetId", "Guid (FK)", "Yes", "References Budget (cascade delete)."],
        ["BucketId", "Guid (FK)", "Yes", "References SpendingBucket (cascade delete)."],
        ["Percentage", "decimal(5,2)", "Yes", "Percentage of total income allocated (e.g., 30.00)."],
        ["AllocatedAmount", "decimal(12,2)", "Yes", "Computed: (Percentage / 100) x Budget.TotalIncome."],
    ],
    col_widths=[1.2, 1.1, 0.8, 2.4],
)

# 2.8
pdf.h3("2.8 ImportBatch  [Done]")
pdf.callout("Changes from original spec: Removed file_size_bytes, bank_format, date_range_start, date_range_end, error_message. Renamed completed_at to ConfirmedAt. Added RolledBackAt. Status simplified to 3 states.", "changed")
pdf.table(
    ["Field", "Type", "Required", "Description"],
    [
        ["Id", "Guid", "PK", "Default: Guid.NewGuid()."],
        ["UserId", "Guid (FK)", "Yes", "References User. DeleteBehavior.Restrict (avoids EF1785)."],
        ["WalletId", "Guid (FK)", "Yes", "References Wallet."],
        ["FileName", "string(255)", "Yes", "Original uploaded file name."],
        ["Status", "ImportBatchStatus", "Yes", "Reviewing, Confirmed, or RolledBack."],
        ["TotalRows", "int", "Yes", "Total rows parsed from the CSV."],
        ["ImportedCount", "int", "Yes", "Count of confirmed transactions."],
        ["DuplicateCount", "int", "Yes", "Rows flagged as duplicates on upload."],
        ["SkippedCount", "int", "Yes", "Rejected transactions after confirm."],
        ["file_size_bytes  [REMOVED]", "int", "--", "Removed from implementation."],
        ["bank_format  [REMOVED]", "enum", "--", "Removed. Generic CSV parser used."],
        ["date_range_start  [REMOVED]", "Date", "--", "Removed."],
        ["date_range_end  [REMOVED]", "Date", "--", "Removed."],
        ["error_message  [REMOVED]", "string", "--", "Removed. Errors returned as HTTP responses."],
        ["CreatedAt", "DateTime", "Auto", "UTC."],
        ["ConfirmedAt  (was: completed_at)", "DateTime?", "No", "Set when batch is confirmed."],
        ["RolledBackAt  [NEW]", "DateTime?", "No", "Set when batch is rolled back."],
    ],
    col_widths=[1.7, 1, 0.8, 2],
)

# 2.9
pdf.h3("2.9 CategorizationRule  [Planned -- Phase 4]")
pdf.callout("This entity is defined in the spec but has not yet been implemented. Planned for Phase 4.", "planned")
pdf.body("Will define pattern-matching rules for automatically assigning categories to imported transactions. Fields remain as originally specified (rule_id, user_id, match_pattern, match_type, subcategory_id, priority, source, match_count, is_active, created_at).")

pdf.h3("2.10 RecurringRule  [Future -- Phase 5]")
pdf.callout("Not yet implemented. The IsRecurring flag and RecurringRuleId on Transaction are placeholders. Fields remain as originally specified.", "planned")

# -- SECTION 3 ----------------------------------------------------------------
pdf.add_page()
pdf.h2("3. Entity Relationship Summary")
pdf.table(
    ["From Entity", "To Entity", "FK Field", "Cardinality"],
    [
        ["Wallet", "User", "UserId", "Many-to-One"],
        ["Transaction", "Wallet", "WalletId", "Many-to-One"],
        ["Transaction", "User", "UserId", "Many-to-One"],
        ["Transaction", "TransactionCategory", "SubcategoryId", "Many-to-One (nullable)"],
        ["Transaction", "ImportBatch", "ImportId", "Many-to-One (nullable)"],
        ["TransactionCategory", "SpendingBucket", "BucketId", "Many-to-One"],
        ["TransactionCategory", "User", "UserId", "Many-to-One (nullable)"],
        ["Budget", "User", "UserId", "Many-to-One"],
        ["BudgetAllocation", "Budget", "BudgetId", "Many-to-One (cascade delete)"],
        ["BudgetAllocation", "SpendingBucket", "BucketId", "Many-to-One (cascade delete)"],
        ["ImportBatch", "User", "UserId", "Many-to-One  (RESTRICT cascade)"],
        ["ImportBatch", "Wallet", "WalletId", "Many-to-One"],
        ["CategorizationRule  [Planned]", "User", "user_id", "Many-to-One (nullable)"],
        ["CategorizationRule  [Planned]", "TransactionCategory", "subcategory_id", "Many-to-One"],
    ],
    col_widths=[1.6, 1.4, 1.2, 1.8],
)
pdf.callout("ImportBatch -> User uses DeleteBehavior.Restrict to avoid SQL Server error 1785 (multiple cascade paths to the Users table).", "changed")

# -- SECTION 4 ----------------------------------------------------------------
pdf.add_page()
pdf.h2("4. CSV Import Specification")

pdf.h3("4.1 Supported Formats")
pdf.callout("Current implementation uses a generic named-column approach. Bank-specific parsers are planned for Phase 5.", "changed")
pdf.table(
    ["Bank / Format", "Status", "Notes"],
    [
        ["Generic (named CSV headers)", "Done", "CSV must have named columns matching the mapping below."],
        ["Chase (auto-detect)", "Planned", "Phase 5."],
        ["Bank of America", "Planned", "Phase 5."],
        ["Wells Fargo", "Planned", "Phase 5."],
    ],
    col_widths=[1.5, 0.8, 3.2],
)

pdf.h3("4.2 CSV Column Mapping (Generic Format)")
pdf.callout("The original spec defined Chase-specific column mapping. Current implementation uses generic named columns via CsvHelper with MissingFieldFound = null (optional columns silently skipped).", "changed")
pdf.table(
    ["CSV Column (header)", "Transaction Field", "Required", "Notes"],
    [
        ["Date", "Date", "Yes", "Parsed as DateOnly."],
        ["Amount", "Amount", "Yes", "Stored as absolute positive value."],
        ["Type", "TransactionType", "Yes", "Income or Expense."],
        ["Description", "Description", "No", "User-facing description."],
        ["RawDescription", "RawDescription", "No", "Original bank description."],
        ["BankTxnType", "BankTxnType", "No", "Bank-provided type label."],
        ["CheckNumber", "CheckNumber", "No", ""],
        ["BalanceAfter", "BalanceAfter", "No", "Decimal. Running balance from bank."],
    ],
    col_widths=[1.3, 1.3, 0.8, 2.1],
)

pdf.h3("4.3 Chase Transaction Type Codes")
pdf.body("For reference when Chase-specific parsing is implemented (Phase 5):")
pdf.table(
    ["Type Code", "Direction", "Description"],
    [
        ["DEBIT_CARD", "Expense", "Point-of-sale card purchase."],
        ["ACH_CREDIT", "Income", "Direct deposit / ACH inflow."],
        ["ACH_DEBIT", "Expense", "Automated payment / ACH outflow."],
        ["LOAN_PMT", "Expense", "Credit card or loan payment."],
        ["ACCT_XFER", "Either", "Internal bank transfer. Exclude from budget."],
        ["QUICKPAY_DEBIT", "Expense", "Zelle outbound."],
        ["QUICKPAY_CREDIT", "Income", "Zelle inbound."],
        ["MISC_DEBIT", "Expense", "Other outflow (investments, transfers)."],
        ["MISC_CREDIT", "Income", "Other inflow."],
    ],
    col_widths=[1.4, 0.9, 3.2],
)

pdf.h3("4.4 Description Cleaning Pipeline")
pdf.body("Applies to Chase-specific parsing (Phase 5). The generic parser stores Description column values directly.")
steps = [
    "Strip trailing dates (e.g., ' 03/23' at end of DEBIT_CARD descriptions).",
    "Extract merchant name for DEBIT_CARD (text before state abbreviation).",
    "Parse ACH descriptions: extract ORIG CO NAME value.",
    "Parse transfer descriptions: extract destination from ACCT_XFER entries.",
    "Parse Zelle descriptions: extract recipient/sender name.",
    "Parse LOAN_PMT: extract card ending digits.",
    "Title-case the result.",
    "Always preserve RawDescription for reference and re-parsing.",
]
for i, s in enumerate(steps, 1):
    pdf.numbered(i, s)
pdf.ln(2)

pdf.h3("4.5 Duplicate Detection Algorithm")
pdf.callout("Hash formula updated: The original spec hashed date + amount + raw_description. The implementation includes walletId as the first component to scope duplicates per wallet.", "changed")
pdf.code(
    'input = "{walletId}|{date:yyyy-MM-dd}|{amount}|{rawDescription?.ToLower().Trim() ?? \"\"}"\n'
    'hash  = SHA256(UTF8.GetBytes(input)).ToHexString().ToLower()   // 64-char hex'
)
pdf.body("Duplicate rows are NOT auto-skipped. They are flagged with IsDuplicate = true and surfaced to the reviewer. The reviewer uses batch approve/reject to decide what to include.")

pdf.h3("4.6 Import Workflow State Machine")
pdf.callout("States simplified: Original spec had 5 states (PENDING, REVIEWING, COMPLETED, ROLLED_BACK, FAILED). Implementation uses 3 states. Parse errors are returned as HTTP 400 before a batch is created.", "changed")
pdf.table(
    ["From", "To", "Trigger", "Action"],
    [
        ["(new)", "Reviewing", "POST /api/import/wallets/{id}", "Batch + pending transactions created. Duplicates flagged."],
        ["Reviewing", "Reviewing", "PATCH approve / reject", "ImportStatus updated on selected transactions."],
        ["Reviewing", "Confirmed", "POST confirm", "Rejected txns soft-deleted. Remaining confirmed. Balance adjusted."],
        ["Confirmed", "RolledBack", "POST rollback", "Balance delta reversed. All confirmed txns soft-deleted."],
    ],
    col_widths=[0.9, 0.9, 1.8, 2.9],
)
pdf.body("A Reviewing batch cannot be rolled back. It must be confirmed first, then rolled back.")

# -- SECTION 5 ----------------------------------------------------------------
pdf.add_page()
pdf.h2("5. Auto-Categorization Engine  [Planned -- Phase 4]")
pdf.callout("The auto-categorization engine is not yet implemented. This section describes the planned design.", "planned")

pdf.h3("5.1 How It Works")
pdf.body("When transactions are imported via CSV, the engine runs each transaction's RawDescription through the CategorizationRule table to attempt automatic category assignment, reducing manual work after each import.")

pdf.h3("5.2 Rule Evaluation")
steps = [
    "Load all active rules for the user, plus all system rules (user_id IS NULL).",
    "Sort by priority DESC, then source rank (USER_CREATED=3 > LEARNED=2 > SYSTEM=1).",
    "For each imported transaction, iterate rules in sorted order.",
    "Apply match based on match_type: CONTAINS, STARTS_WITH, EXACT, REGEX.",
    "First matching rule wins. Set SubcategoryId; increment match_count.",
    "If no rule matches, leave SubcategoryId null and flag for manual review.",
]
for i, s in enumerate(steps, 1):
    pdf.numbered(i, s)
pdf.ln(2)

pdf.h3("5.3 Learning from User Actions")
pdf.body("When a user manually categorizes an uncategorized transaction, the system will prompt: \"Always categorize transactions containing 'LA FITNESS' as Health & Fitness?\" If accepted, a new rule with source = LEARNED is created using the cleaned description keyword.")

pdf.h3("5.4 System Default Rules")
pdf.table(
    ["Pattern", "Match Type", "Category", "Bucket"],
    [
        ["NETFLIX", "CONTAINS", "Streaming Services", "Lifestyle"],
        ["LA FITNESS", "CONTAINS", "Gym & Fitness", "Lifestyle"],
        ["LINKEDIN", "CONTAINS", "Professional Subscriptions", "Lifestyle"],
        ["REMITLY", "CONTAINS", "Remittance", "Other"],
        ["PAYROLL", "CONTAINS", "Salary", "Income"],
        ["WEALTHFRONT", "CONTAINS", "Investments", "Savings & Investments"],
        ["SOFI", "CONTAINS", "Savings Transfer", "Savings & Investments"],
        ["Payment to Chase card", "STARTS_WITH", "Credit Card Payment", "Debt"],
        ["STUDENT LOAN", "CONTAINS", "Student Loan", "Debt"],
        ["Zelle payment", "STARTS_WITH", "Peer Payment", "Other"],
    ],
    col_widths=[1.5, 1, 1.3, 1.7],
)

# -- SECTION 6 ----------------------------------------------------------------
pdf.add_page()
pdf.h2("6. API Endpoint Specification")
pdf.callout("URL change: All routes use /api/ -- not /api/v1/ as originally specified. Authentication via Authorization: Bearer <token> extracted from the app.at cookie.", "changed")

pdf.h3("6.1 Authentication -- /api/authentication")
pdf.callout("Route changed: Original was /api/v1/auth/. Implemented as /api/authentication/. Login endpoint is /authenticate (not /login). Refresh and password-reset endpoints are not yet implemented.", "changed")
pdf.table(
    ["Method", "Endpoint", "Auth", "Description"],
    [
        ["POST", "/api/authentication/register", "No", "Register. Body: { firstName, lastName, email, password, currency }."],
        ["POST", "/api/authentication/authenticate", "No", "Login. Sets app.at cookie. Body: { email, password }."],
        ["POST  [PLANNED]", "/api/authentication/refresh", "Yes", "Refresh expiring JWT token."],
        ["POST  [PLANNED]", "/api/authentication/forgot-password", "No", "Initiate password reset via email."],
        ["POST  [PLANNED]", "/api/authentication/reset-password", "No", "Complete password reset with token."],
    ],
    col_widths=[0.9, 2, 0.6, 2.5],
)

pdf.h3("6.2 Wallets -- /api/wallets")
pdf.table(
    ["Method", "Endpoint", "Description"],
    [
        ["GET", "/api/wallets", "Get authenticated user + their wallets array."],
        ["POST", "/api/wallets", "Create wallet. Body: { walletName, walletType, institution, currency, balance }."],
        ["GET", "/api/wallets/{id}", "Get wallet details."],
        ["PUT", "/api/wallets/{id}", "Update wallet fields."],
        ["DELETE", "/api/wallets/{id}", "Soft-delete wallet."],
    ],
    col_widths=[0.7, 1.6, 3.2],
)

pdf.h3("6.3 Transactions -- /api/transactions")
pdf.table(
    ["Method", "Endpoint", "Description"],
    [
        ["GET", "/api/transactions?walletId={guid}", "List transactions for a wallet. Excludes soft-deleted."],
        ["POST", "/api/transactions", "Create manual transaction. Adjusts wallet balance. Body: { walletId, amount, transactionType, date, description, subcategoryId }."],
        ["GET", "/api/transactions/{id}", "Get single transaction detail."],
        ["PUT", "/api/transactions/{id}", "Partial update. Null fields skipped via AutoMapper PreCondition."],
        ["DELETE", "/api/transactions/{id}", "Soft-delete. Reverses wallet balance adjustment."],
    ],
    col_widths=[0.7, 2, 2.8],
)

pdf.h3("6.4 Transaction Categories -- /api/transactioncategories")
pdf.callout("Route is /api/transactioncategories (not /api/categories as in the original spec).", "changed")
pdf.table(
    ["Method", "Endpoint", "Description"],
    [
        ["GET", "/api/transactioncategories", "List all categories: system + user-owned. Excludes soft-deleted."],
        ["GET", "/api/transactioncategories/{id}", "Get single category."],
        ["POST", "/api/transactioncategories", "Create user category. Body: { bucketId, categoryName, isFixed, icon, color }."],
        ["PUT", "/api/transactioncategories/{id}", "Update category fields."],
        ["DELETE", "/api/transactioncategories/{id}", "Soft-delete. System categories are blocked from deletion."],
    ],
    col_widths=[0.7, 2, 2.8],
)

pdf.add_page()
pdf.h3("6.5 CSV Import -- /api/import")
pdf.callout("Route and flow changed from original spec. Old: /api/v1/imports/ with preview endpoints. New: /api/import/ with streamlined upload -> review -> approve/reject -> confirm/rollback flow.", "changed")
pdf.table(
    ["Method", "Endpoint", "Description"],
    [
        ["POST", "/api/import/wallets/{walletId}", "Upload CSV (multipart/form-data, field: file). Returns 201 ImportBatchDto."],
        ["GET", "/api/import", "List all import batches for authenticated user."],
        ["GET", "/api/import/{id}", "Get batch detail: transactions, pendingDuplicates, readyToConfirm."],
        ["PATCH", "/api/import/{id}/transactions/approve", "Bulk-approve transactions. Body: { transactionIds: [guid,...] }."],
        ["PATCH", "/api/import/{id}/transactions/reject", "Bulk-reject transactions. Body: { transactionIds: [guid,...] }."],
        ["POST", "/api/import/{id}/confirm", "Finalize: soft-delete rejected, confirm remaining, adjust wallet balance."],
        ["POST", "/api/import/{id}/rollback", "Reverse confirmed batch: reverse balance, soft-delete all confirmed txns."],
        ["GET  [REMOVED]", "/api/v1/imports/:id/preview", "Removed. Combined into GET /api/import/{id}."],
        ["PATCH  [REMOVED]", "/api/v1/imports/:id/preview", "Removed. Replaced by bulk approve/reject endpoints."],
    ],
    col_widths=[0.9, 2, 2.6],
)

pdf.h3("6.6 Budgets -- /api/budgets")
pdf.table(
    ["Method", "Endpoint", "Description"],
    [
        ["GET", "/api/budgets", "List all budgets for authenticated user."],
        ["POST", "/api/budgets", "Create budget. Body: { name, totalIncome, startDate, endDate, isActive }."],
        ["GET", "/api/budgets/{id}", "Get budget with its allocations."],
        ["PUT", "/api/budgets/{id}", "Update budget fields."],
        ["DELETE", "/api/budgets/{id}", "Soft-delete budget."],
        ["GET", "/api/budgets/{id}/allocations", "List allocations for a budget."],
        ["POST", "/api/budgets/{id}/allocations", "Create allocation. Body: { bucketId, percentage, allocatedAmount }. Validates total <= 100%."],
        ["GET", "/api/budgets/{id}/allocations/{aid}", "Get single allocation."],
        ["PUT", "/api/budgets/{id}/allocations/{aid}", "Update allocation."],
        ["DELETE", "/api/budgets/{id}/allocations/{aid}", "Delete allocation."],
        ["GET  [PLANNED]", "/api/budgets/{id}/progress", "Budget vs actual comparison with overspend alerts."],
    ],
    col_widths=[0.9, 2, 2.6],
)

pdf.h3("6.7 Spending Buckets")
pdf.callout("No CRUD endpoints yet. SpendingBuckets are seeded at startup. User-created custom buckets and bucket CRUD are planned for Phase 5.", "changed")

pdf.h3("6.8 Categorization Rules  [Planned -- Phase 4]")
pdf.callout("Not yet implemented. Routes will be under /api/rules.", "planned")
pdf.table(
    ["Method", "Endpoint", "Description"],
    [
        ["GET", "/api/rules", "List all rules (user + system)."],
        ["POST", "/api/rules", "Create a rule."],
        ["PATCH", "/api/rules/{id}", "Update rule."],
        ["DELETE", "/api/rules/{id}", "Delete rule."],
        ["POST", "/api/rules/test", "Test a pattern against a description."],
    ],
    col_widths=[0.7, 1.5, 3.3],
)

# -- SECTION 7 ----------------------------------------------------------------
pdf.add_page()
pdf.h2("7. Use Case Catalog")

pdf.h3("7.1 User Account Management")
ucs = [
    "UC-001: Register new account with email, first name, last name, password, and preferred currency.",
    "UC-002: Sign in with email and password. JWT delivered via app.at cookie.",
    "UC-003: Reset forgotten password via email link.  [Planned]",
    "UC-004: Update profile (name, currency, settings).  [Planned]",
    "UC-005: Delete account.  [Planned]",
]
for u in ucs: pdf.bullet(u)

pdf.h3("7.2 Wallet Management")
ucs = [
    "UC-010: Create a new wallet specifying name, type, institution, currency, and initial balance.",
    "UC-011: View all wallets with current balances.",
    "UC-012: Update wallet details (name, institution).",
    "UC-013: Soft-delete a wallet.",
]
for u in ucs: pdf.bullet(u)

pdf.h3("7.3 Transaction Management")
ucs = [
    "UC-020: Add a manual transaction with amount, date, type, description, wallet, and category.",
    "UC-021: View transactions filtered by wallet.",
    "UC-022: View transaction detail including raw and clean descriptions.",
    "UC-023: Update a transaction's category, description, amount, or date.",
    "UC-024: Soft-delete a transaction. Wallet balance recalculates.",
    "UC-025: Bulk-categorize multiple transactions.  [Planned]",
]
for u in ucs: pdf.bullet(u)

pdf.h3("7.4 CSV Import")
ucs = [
    "UC-030: Upload a CSV bank statement file targeting a specific wallet.",
    "UC-031: System parses CSV, computes duplicate hashes, and flags matches.",
    "UC-032: Review batch: see all transactions, duplicate count, pendingDuplicates.",
    "UC-033: Bulk-approve duplicate-flagged transactions to include them in the import.",
    "UC-034: Bulk-reject duplicate-flagged transactions to exclude them.",
    "UC-035: Confirm batch: wallet balance adjusts, rejected transactions soft-deleted.",
    "UC-036: View import history showing all past batches with stats.",
    "UC-037: Rollback a confirmed import: balance reversed, all confirmed txns soft-deleted.",
    "UC-038: Auto-categorize transactions using CategorizationRule engine.  [Planned]",
    "UC-039: Prompt to create a learned rule when user manually categorizes.  [Planned]",
]
for u in ucs: pdf.bullet(u)

pdf.h3("7.5 Budget Planning")
ucs = [
    "UC-040: Create a named budget with total income, date range, and percentage allocations.",
    "UC-041: View budget allocations per spending bucket.",
    "UC-042: View budget vs actual spending per bucket.  [Planned]",
    "UC-043: Receive overspend alert when a bucket exceeds allocation.  [Planned]",
    "UC-044: Edit budget: adjust percentages, income, or date range.",
    "UC-045: Duplicate a previous budget as a template.  [Planned]",
]
for u in ucs: pdf.bullet(u)

pdf.h3("7.6 Categorization Management")
ucs = [
    "UC-050: Create a custom transaction category under a spending bucket.",
    "UC-051: Tag a category as fixed or variable expense.",
    "UC-052: View, create, edit, and delete auto-categorization rules.  [Planned]",
    "UC-053: Test a rule pattern against sample descriptions.  [Planned]",
]
for u in ucs: pdf.bullet(u)

# -- SECTION 8 ----------------------------------------------------------------
pdf.add_page()
pdf.h2("8. Database Indexes & Constraints")

pdf.h3("8.1 Implemented Indexes")
pdf.table(
    ["Table", "Index Columns", "Purpose"],
    [
        ["Transactions", "(UserId, Date)", "Primary listing query by date."],
        ["Transactions", "(WalletId, Date)", "Wallet-scoped transaction listing."],
        ["Transactions", "DuplicateHash", "Import deduplication lookup."],
        ["Budgets", "(UserId) WHERE IsActive = 1", "Partial unique index -- one active budget per user."],
    ],
    col_widths=[1, 1.5, 3],
)

pdf.h3("8.2 Key Constraints")
pdf.bullet("Budgets: Partial unique index on (UserId) filtered by IsActive = 1. Database-level via EF Core migration.")
pdf.bullet("BudgetAllocations: Application-level check that SUM(Percentage) <= 100% per budget. (Not required to equal exactly 100%.)")
pdf.bullet("Transactions.Amount: Always stored as a positive decimal. Direction determined by TransactionType.")
pdf.bullet("ImportBatch -> User (UserId): DeleteBehavior.Restrict to prevent SQL Server cascade cycle error 1785.")

# -- SECTION 9 ----------------------------------------------------------------
pdf.add_page()
pdf.h2("9. Seed Data")

pdf.h3("9.1 Default Spending Buckets")
pdf.table(
    ["Bucket Name", "Description"],
    [
        ["Housing", "Rent, mortgage, property tax, home insurance, HOA fees."],
        ["Transportation", "Car payment, gas, insurance, public transit, ride-share."],
        ["Lifestyle", "Groceries, dining, entertainment, personal care, subscriptions."],
        ["Debt", "Credit card payments, student loans, personal loans."],
        ["Savings & Investments", "Savings transfers, 401(k), IRA, brokerage deposits."],
        ["Income", "Salary, freelance, side hustles, interest income."],
        ["Other", "Uncategorized, miscellaneous, remittances, peer payments."],
    ],
    col_widths=[1.4, 4.1],
)

pdf.h3("9.2 Default Transaction Categories")
pdf.table(
    ["Bucket", "Category Name", "Is Fixed", "Example Transactions"],
    [
        ["Housing", "Rent / Mortgage", "Yes", "Monthly rent, mortgage payment."],
        ["Housing", "Utilities", "Yes", "Electric, water, gas, internet."],
        ["Housing", "Home Insurance", "Yes", "Annual or monthly premium."],
        ["Transportation", "Car Payment", "Yes", "Auto loan installment."],
        ["Transportation", "Gas", "No", "Fuel purchases."],
        ["Transportation", "Auto Insurance", "Yes", "Monthly premium."],
        ["Lifestyle", "Groceries", "No", "Supermarket, grocery delivery."],
        ["Lifestyle", "Dining Out", "No", "Restaurants, coffee shops, fast food."],
        ["Lifestyle", "Entertainment", "No", "Movies, concerts, gaming."],
        ["Lifestyle", "Streaming Services", "Yes", "Netflix, Spotify, Disney+."],
        ["Lifestyle", "Gym & Fitness", "Yes", "Gym membership, fitness apps."],
        ["Lifestyle", "Personal Care", "No", "Haircuts, salon, grooming."],
        ["Debt", "Credit Card Payment", "No", "Monthly CC payments."],
        ["Debt", "Student Loan", "Yes", "Monthly loan payment."],
        ["Savings & Investments", "Savings Transfer", "No", "Transfers to savings accounts."],
        ["Savings & Investments", "Investment Deposit", "No", "Brokerage, Wealthfront, M1."],
        ["Income", "Salary", "Yes", "Regular paycheck."],
        ["Income", "Freelance", "No", "Contract or gig income."],
        ["Other", "Remittance", "No", "International money transfers (Remitly)."],
        ["Other", "Peer Payment", "No", "Zelle, Venmo, Cash App."],
    ],
    col_widths=[1.3, 1.3, 0.7, 2.2],
)

# -- SECTION 10 ----------------------------------------------------------------
pdf.add_page()
pdf.h2("10. Implementation Roadmap")

pdf.h3("Phase 1: Foundation  [Complete]")
pdf.table(
    ["#", "Task", "Status"],
    [
        ["1.1", "Project setup: EF Core, SQL Server, migrations", "Done"],
        ["1.2", "Seed data: default buckets + categories", "Done"],
        ["1.3", "User registration & authentication (JWT cookie)", "Done"],
        ["1.4", "User profile CRUD", "Done"],
        ["1.5", "Wallet CRUD", "Done"],
        ["1.6", "Transaction CRUD (manual entry, balance adjustment)", "Done"],
        ["1.7", "SpendingBucket (seeded, no CRUD endpoints yet)", "Done"],
        ["1.8", "TransactionCategory CRUD (system + user-scoped, soft delete)", "Done"],
    ],
    col_widths=[0.5, 3.8, 1.2],
)

pdf.h3("Phase 2: Budget Planning  [Complete]")
pdf.table(
    ["#", "Task", "Status"],
    [
        ["2.1", "Budget CRUD (one-active-per-user constraint)", "Done"],
        ["2.2", "BudgetAllocation CRUD (nested under budget, <= 100% validation)", "Done"],
        ["2.3", "Budget vs Actual aggregation query", "Planned"],
        ["2.4", "Overspend detection per-bucket", "Planned"],
        ["2.5", "Budget duplication (copy as template)", "Planned"],
    ],
    col_widths=[0.5, 3.8, 1.2],
)

pdf.h3("Phase 3: CSV Import  [Complete]")
pdf.table(
    ["#", "Task", "Status"],
    [
        ["3.1", "Generic CSV parser (CsvHelper, MissingFieldFound=null)", "Done"],
        ["3.2", "ImportBatch entity, migration, status tracking", "Done"],
        ["3.3", "Duplicate detection via SHA-256 hash (includes walletId)", "Done"],
        ["3.4", "IsDuplicate + ImportStatus fields added to Transaction", "Done"],
        ["3.5", "Batch approve / reject endpoints (PATCH)", "Done"],
        ["3.6", "Confirm endpoint (balance adjustment, soft-delete rejects)", "Done"],
        ["3.7", "Rollback endpoint (delta reversal, soft-delete confirmed)", "Done"],
        ["3.8", "Import history listing", "Done"],
        ["3.9", "Chase-specific parser with header detection + description cleaning", "Planned"],
    ],
    col_widths=[0.5, 3.8, 1.2],
)

pdf.h3("Phase 4: Smart Categorization  [Planned]")
pdf.table(
    ["#", "Task", "Status"],
    [
        ["4.1", "CategorizationRule entity and migration", "Planned"],
        ["4.2", "Rule evaluation engine (match imported transactions)", "Planned"],
        ["4.3", "Seed system default rules (Netflix, payroll, etc.)", "Planned"],
        ["4.4", "Integrate auto-categorization into import upload", "Planned"],
        ["4.5", "Learn from user corrections (prompt + create LEARNED rule)", "Planned"],
        ["4.6", "Rule CRUD + test endpoint", "Planned"],
    ],
    col_widths=[0.5, 3.8, 1.2],
)

pdf.h3("Phase 5: Polish & Extend  [Future]")
pdf.table(
    ["#", "Task", "Status"],
    [
        ["5.1", "Chase CSV parser (auto-detect headers, description cleaning)", "Future"],
        ["5.2", "Bank of America CSV parser", "Future"],
        ["5.3", "Wells Fargo CSV parser", "Future"],
        ["5.4", "RecurringRule entity + transaction auto-generation", "Future"],
        ["5.5", "Budget vs Actual monthly report", "Future"],
        ["5.6", "Category spending trends over time", "Future"],
        ["5.7", "SpendingBucket CRUD (user-created custom buckets)", "Future"],
        ["5.8", "Data export (CSV and PDF)", "Future"],
        ["5.9", "Transfer linking (pair send/receive transactions)", "Future"],
    ],
    col_widths=[0.5, 3.8, 1.2],
)

# -- SECTION 11 ----------------------------------------------------------------
pdf.add_page()
pdf.h2("11. Appendix")

pdf.h3("11.1 Enum Definitions")
pdf.table(
    ["Enum", "Values", "Notes"],
    [
        ["WalletType", "Checking, Savings, CreditCard, Cash, Loan, Investment", "Stored as string in DB"],
        ["Currency", "USD, EUR, GBP, CAD, AUD, JPY, CHF, CNY, INR, MXN", "Stored as string in DB"],
        ["TransactionType", "Income, Expense", "Stored as string in DB"],
        ["TransactionSource", "Manual, Import", "Stored as string in DB"],
        ["ImportBatchStatus", "Reviewing, Confirmed, RolledBack", "Simplified from original 5-state model"],
        ["ImportStatus", "Pending, Confirmed, Rejected", "Per-transaction import status -- new in Phase 2.8"],
        ["MatchType", "CONTAINS, STARTS_WITH, EXACT, REGEX", "Planned -- CategorizationRule"],
        ["RuleSource", "SYSTEM, USER_CREATED, LEARNED", "Planned -- CategorizationRule"],
        ["Frequency", "DAILY, WEEKLY, BIWEEKLY, MONTHLY, QUARTERLY, YEARLY", "Future -- RecurringRule"],
    ],
    col_widths=[1.3, 2.3, 1.9],
)

pdf.h3("11.2 Wallet Type Reference")
pdf.table(
    ["Wallet Type", "Examples", "Notes"],
    [
        ["Checking", "Chase Checking, everyday debit", "Primary transaction wallet. Supports all types."],
        ["Savings", "High-yield savings, emergency fund", "Typically receives transfers."],
        ["CreditCard", "Chase Sapphire, Amex Gold", "Expenses only. Payments show as LOAN_PMT in checking."],
        ["Cash", "Cash on hand, petty cash", "Manual tracking only. No CSV import."],
        ["Loan", "Student loan, mortgage", "Tracks debt balance."],
        ["Investment", "401(k), IRA, Wealthfront, M1", "Tracks investment contributions and value."],
    ],
    col_widths=[1, 1.6, 2.9],
)

pdf.h3("11.3 Design Decisions & Gotchas")
pdf.table(
    ["Decision", "Detail"],
    [
        ["AutoMapper PreCondition", "Use PreCondition (not Condition) for nullable value type fields in Update DTOs to skip mapping when source is null."],
        ["Never call .Update() on tracked entities", "After FindAsync() / FirstOrDefaultAsync() the entity is already tracked. Modify properties and call SaveChangesAsync()."],
        ["Bulk updates via ExecuteUpdateAsync", "Used for batch approve/reject to update many rows without loading entities into memory."],
        ["Scoped repositories share DbContext", "Repositories are Scoped; Services are Transient. Changes by one repo are visible to another within the same request."],
        ["Delta-based balance updates", "Wallet balance is adjusted by a delta (+/- amount) -- never recalculated from all transactions. Rollback reverses only the delta of confirmed transactions."],
        ["FK cascade restriction", "ImportBatch -> User (UserId) uses DeleteBehavior.Restrict to avoid SQL Server error 1785 (multiple cascade paths)."],
        ["Soft deletes", "Entities use DeletedAt timestamp. All queries filter DeletedAt == null."],
    ],
    col_widths=[1.5, 4],
)

pdf.h3("11.4 Sample Chase CSV Observations")
pdf.body("Key patterns from Chase9275_Activity_20260330.CSV (66 transactions, Jan-Mar 2026):")
notes = [
    "Payroll deposits are bi-weekly ACH_CREDIT entries with 'TEXAS PRE PAYROLL' in description. Amount: $2,310.87.",
    "Investment transfers to Wealthfront and M1 Finance appear as MISC_DEBIT -- default to Savings & Investments.",
    "SoFi Bank appears as both MISC_DEBIT (outgoing) and MISC_CREDIT (incoming), indicating cross-bank movement.",
    "Remitly transactions use both DEBIT_CARD and MISC_DEBIT types. Match on 'REMITLY' or 'RMTLY'.",
    "Credit card payments (LOAN_PMT) reference card ending digits like '1850' and '6479'.",
    "Internal transfers (ACCT_XFER) reference 'CHK ...4060'. Exclude from budget calculations.",
    "Balance column may be empty on recent rows -- parser must handle missing balance gracefully.",
]
for n in notes:
    pdf.bullet(n)

# -- OUTPUT -------------------------------------------------------------------
pdf.output(OUT)
print(f"PDF generated: {OUT}")

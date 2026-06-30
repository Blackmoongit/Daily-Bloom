# 🌸 Daily Bloom

A cute, cozy personal dashboard built with **Blazor Server (.NET 8)** that combines a To‑Do list, a budget tracker, and a reading list — with its own user accounts.

---

## 1. What's inside

```
DailyBloom/
├── DailyBloom.csproj          # project file + NuGet packages
├── Program.cs                 # app startup: DB, auth, routing
├── appsettings.json           # SQLite connection string
├── Models/                    # plain C# classes = your data shapes
│   ├── User.cs
│   ├── TaskItem.cs            # TaskItem, SubTask, TaskComment, TaskAttachment
│   ├── Budget.cs              # IncomeEntry, ExpenseEntry
│   └── Book.cs
├── Data/
│   └── AppDbContext.cs        # EF Core — maps Models to SQLite tables
├── Services/
│   ├── AuthService.cs         # register / login logic + password hashing
│   └── CurrentUserService.cs  # tracks who is logged in (per browser tab)
├── Components/
│   ├── App.razor              # the single HTML page that hosts Blazor
│   ├── Routes.razor           # router + login-guard for protected pages
│   ├── _Imports.razor         # shared `@using` statements
│   ├── Layout/
│   │   ├── MainLayout.razor   # sidebar with the 3 nav tabs
│   │   └── RedirectToLogin.razor
│   └── Pages/
│       ├── Login.razor
│       ├── Register.razor
│       ├── DashboardRedirect.razor
│       ├── TodoPage.razor     # To-Do List tab
│       ├── BudgetPage.razor   # Budgeting tab
│       └── BooksPage.razor    # Books tab
└── wwwroot/
    ├── css/app.css            # the pastel "cute vibe" theme
    └── uploads/               # profile pictures, book covers, task files get saved here
```

---

## 2. How the pieces fit together (step by step)

### Step 1 — Models (`Models/`)
These are just plain classes describing the shape of your data — a `User` has a name, username, password hash, etc.; a `TaskItem` has a title, priority, end date, and lists of `SubTask`, `TaskComment`, and `TaskAttachment`. `IncomeEntry`/`ExpenseEntry` cover Budgeting, and `Book` covers the reading list (with `IsRead` as the "Will Read → Read" checkbox flag).

### Step 2 — Database (`Data/AppDbContext.cs`)
This is Entity Framework Core's `DbContext`. It exposes a `DbSet<T>` for every model — these behave like in‑memory tables you can query with LINQ, and EF Core translates that into SQL against a local **SQLite** file (`App_Data/dailybloom.db`). `OnModelCreating` sets up cascading deletes (so deleting a task also deletes its sub‑tasks/comments/attachments) and a unique index on `Username`.

### Step 3 — Authentication (`Services/AuthService.cs` + `CurrentUserService.cs`)
- `AuthService.RegisterAsync` checks the username isn't taken, hashes the password with **BCrypt** (never stored in plain text), and saves a new `User` row.
- `AuthService.LoginAsync` looks up the user and verifies the password hash.
- `CurrentUserService` remembers *who's logged in* using the browser's encrypted session storage (`ProtectedSessionStorage`). This is simpler than cookie-based auth for an interactive Blazor Server app, and it plugs into Blazor's `AuthenticationStateProvider` so `[Authorize]` and `<AuthorizeView>` work normally.

### Step 4 — Routing & layout
- `Routes.razor` wraps every page in `<AuthorizeRouteView>` — if a page is marked `@attribute [Authorize]` and nobody is logged in, it bounces the user to `/login`.
- `MainLayout.razor` is the sidebar shell with your **3 nav tabs**: To‑Do List, Budgeting, Books — plus the logged-in user's avatar/name and a logout button.

### Step 5 — Pages (the actual features)

**`TodoPage.razor`** (To‑Do List)
- A dropdown filters tasks by **Day / Week / Month / Year / All**, and a search box filters by title/description/hashtag.
- The left column lists task cards (priority color stripe, due date, hashtags, checkbox, sub‑task checklist).
- The right column is a **calendar sidebar** grouping the currently-filtered tasks by their end date.
- Clicking "+ Add Task" or the ✏️ icon opens a modal where you edit title, description, priority, end date, hashtags, add/remove sub‑tasks, add comments, and upload file attachments (saved under `wwwroot/uploads/tasks`).

**`BudgetPage.razor`** (Budgeting)
- A month picker (auto-built from your existing entries + the current month).
- Three summary cards: **Total Income**, **Total Expense**, **Grand Total** (income − expense) for the selected month.
- Two columns list Income and Expense entries (date, description, amount, comments) with edit/delete, and a modal to add/edit either type.

**`BooksPage.razor`** (Books)
- Two sections: **Will Read** and **Read**.
- New books always start in "Will Read". Ticking the checkbox on a book card flips `IsRead` to `true` and it moves to "Read".
- Each book can have a name, optional cover image upload, a star rating (0–5), and a reflection/comment text box.

### Step 6 — Styling (`wwwroot/css/app.css`)
One stylesheet with CSS variables for the pastel pink/lavender/mint palette, rounded cards, soft shadows, and a sidebar layout — applied consistently across auth pages, the dashboard, modals, and cards to keep the "cute" feel uniform.

---

## 3. Running it locally

You'll need the **.NET 8 SDK** installed (https://dotnet.microsoft.com/download).

```bash
cd DailyBloom
dotnet restore
dotnet run
```

Then open the URL shown in the terminal (usually `https://localhost:5001` or similar). The SQLite database file and `uploads` folder are created automatically on first run — no manual DB setup needed.

To reset all data, just delete `App_Data/dailybloom.db` and restart.

---

## 4. Notes & possible next steps

- **Auth model**: login state is stored per browser tab via encrypted session storage rather than a persistent cookie, so it's simple and self-contained but won't "remember" you after closing the tab. If you want "stay signed in for 30 days," swap `CurrentUserService` for standard ASP.NET Core cookie authentication (`AddAuthentication().AddCookie()` + `SignInAsync` in a small non-Blazor login endpoint).
- **File uploads** are capped at 5–10MB and saved to `wwwroot/uploads/...`; for production you'd typically move these to cloud storage (Azure Blob, S3, etc.) instead of the local disk.
- **Validation**: forms currently rely on simple required-field checks; you can tighten this with `[Required]`/`[StringLength]` data annotations + `<DataAnnotationsValidator />` (already wired up on Login/Register) and add the same to the modal forms.
- **Calendar view**: the sidebar currently lists upcoming tasks grouped by date (agenda-style) rather than a full month grid — that keeps it lightweight, but a true month-grid calendar could be added with a small custom component if you'd like.

Enjoy your bloom! 🌷

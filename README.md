# 22-48044-2_LoginSystem

A WinForms desktop application implementing Registration, Login, and Logout
against a SQL Server database, built for Lab 1 (Login, Registration & Logout
with C# and SQL Server).

## Environment

- **SQL Server edition:** _[FILL IN — e.g. SQL Server Express 2022 / Developer / LocalDB]_
- **Visual Studio version:** _[FILL IN — e.g. Visual Studio 2022 17.x]_
- **.NET version:** _[FILL IN — e.g. .NET Framework 4.7.2 / .NET 8 WinForms]_
- **Connection string format** (App.config, no real passwords committed):
  ```xml
  <connectionStrings>
    <add name="LoginDB"
         connectionString="Data Source=localhost;Initial Catalog=22-48044-2_LoginDB;Integrated Security=True;Connect Timeout=30"
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  ```

## Database

The database `22-48044-2_LoginDB` and the `dbo.Users` table are created by
[`Schema.sql`](./Schema.sql), run as a query in SQL Server Object Explorer /
SSMS. The table has the following columns:

| Column       | Type          | Nullable |
|--------------|---------------|----------|
| UserID       | INT IDENTITY  | NO (PK)  |
| Username     | NVARCHAR(50)  | NO (UNIQUE) |
| PasswordHash | NVARCHAR(200) | NO       |
| Email        | NVARCHAR(100) | YES      |
| FullName     | NVARCHAR(100) | YES      |
| CreatedAt    | DATETIME      | NO (default GETDATE()) |

## How Registration, Login, and Logout work

- **Registration (`RegisterForm.cs`)** — validates that no field is empty,
  the password is at least 6 characters, both password fields match, and the
  email contains `@`. It then checks for a duplicate username with
  `ExecuteScalar()`, hashes the password with SHA-256, and inserts the new
  row using a parameterized `ExecuteNonQuery()`.
- **Login (`LoginForm.cs`)** — looks up the user by username with a
  parameterized query, hashes the entered password, and compares hashes
  (never plain text). On success it opens `HomeForm` and hides the login
  window. On failure it shows a message and disables the Login button after
  3 failed attempts.
- **Logout (`HomeForm.cs`)** — closes `HomeForm` and returns to a freshly
  cleared `LoginForm` (empty fields, focus on username). The application
  does not exit and no orphan forms are left running.
- **DatabaseHelper.cs** — _[FILL IN if you used this bonus: describe how DB
  code was centralized here instead of living in the forms]_

## Password hashing

Passwords are hashed with SHA-256 before being stored; the plain-text
password is never written to the database. At login, the entered password
is hashed the same way and the two hashes are compared. Storing plain text
would mean anyone with read access to the database (or a leaked backup)
could see every user's real password immediately, and because people reuse
passwords across sites, that exposure isn't limited to this one app.

## SQL Injection demonstration

- **Vulnerable version:** built its query by string concatenation, e.g.
  `"SELECT * FROM dbo.Users WHERE Username='" + username + "' AND PasswordHash='" + passwordHash + "'"`.
  Submitting `' OR '1'='1` as input makes the WHERE clause always true,
  returning every row and logging in with no valid password.
- **Fixed version:** the real `LoginForm`/`DatabaseHelper` code uses a
  parameterized query (`@username`, `@passwordHash`), so the same input is
  sent to SQL Server as a literal value, never as SQL syntax, and is
  correctly rejected as a wrong password.
- **Why parameters stop it:** the query plan is compiled from the SQL text
  alone; parameter values are sent separately and substituted afterward, so
  user input can never change the structure or logic of the query.

Screenshots of both the vulnerable and fixed behavior are in `Report.pdf`.

## Bonus tasks attempted

_[FILL IN — e.g. "None" / "LoginHistory table" / "Change-password screen" /
"Moved all DB code into DatabaseHelper.cs" — list whichever two, or none]_

## Screenshots

All required screenshots (table design, registration, successful login,
failed login/lockout, home screen with grid, logout, injection demo
before/after) are included in `Report.pdf`.

## Problems encountered and how they were solved

_[FILL IN — e.g. any connection string issues, LocalDB errors, unique
constraint errors during testing, etc., and how you fixed them]_

## Notes on the sample project's bugs (for viva)

The provided `Login_System` sample project was **not** used or edited. It
contains, among other issues: SQL injection via string concatenation, two
conflicting connection strings in one form, a code/schema table-name
mismatch (`LoginMst` vs `Table`), an unused/never-closed connection in
`Form1_Load`, no try/catch around database calls, `con.Close()` outside a
`finally`/`using` block, plain-text password storage and comparison, a
missing space producing `'x'and` in concatenated SQL, opening a website
instead of a home screen on success, non-descriptive control names
(`button1`, `textBox1`), and no registration form or logout at all despite
the project's name.

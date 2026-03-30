# AttireZone Authentication - Developer Reference

## Quick Reference Guide

This guide provides developer-level reference for the authentication system implementation.

---

## Code Architecture

### Layer Structure

```
Presentation Layer (UI)
    ↓
    Register.aspx / Login.aspx
    ↓
Application Layer (Logic)
    ↓
    UserService.cs (BusinessLogic)
    ↓
Data Access Layer
    ↓
    DBHelper.cs → SQL Server Database
```

---

## Key Files Overview

### 1. Register.aspx

**Location**: `Auth/Register.aspx`

**Key Controls**:

- `txtFullName` - Full name input
- `txtEmail` - Email input
- `txtPassword` - Password input
- `txtConfirmPassword` - Confirm password input
- `btnRegister` - Submit button
- `alertPlaceholder` - Alert messages container
- `hlkLogin` - Link to login page

**Event Handler**:

```csharp
btnRegister_Click(object sender, EventArgs e)
```

### 2. Register.aspx.cs

**Location**: `Auth/Register.aspx.cs`

**Namespace**: `AttireZone_Web_App.Auth`
**Class**: `Register : Page`

**Methods**:

```csharp
protected void Page_Load(object sender, EventArgs e)
protected void btnRegister_Click(object sender, EventArgs e)
private void ShowSuccessAlert(string message)
private void ShowErrorAlert(string message)
```

**Validation Flow**:

1. `Page_Load` - Check if already logged in
2. `btnRegister_Click` - Validate form
3. Call `UserService.RegisterUser()`
4. Display success/error alert
5. Redirect to login

**Example Usage**:

```csharp
bool success = UserService.RegisterUser(fullName, email, password);
if (success) {
    ShowSuccessAlert("Registration successful!");
} else {
    ShowErrorAlert("Registration failed!");
}
```

### 3. Login.aspx.cs

**Location**: `Auth/Login.aspx.cs`

**Namespace**: `AttireZone_Web_App.Auth`
**Class**: `Login : Page`

**Methods**:

```csharp
protected void Page_Load(object sender, EventArgs e)
protected void btnLogin_Click(object sender, EventArgs e)
private void ShowSuccessAlert(string message)
private void ShowErrorAlert(string message)
```

**Validation Flow**:

1. `Page_Load` - Check if already logged in
2. `btnLogin_Click` - Validate form
3. Call `UserService.AuthenticateUser()`
4. Set session variables
5. Set remember-me cookie (if checked)
6. Display success/error alert
7. Redirect to home page

**Example Usage**:

```csharp
User user = UserService.AuthenticateUser(email, password);
if (user != null) {
    Session["UserId"] = user.UserId;
    Session["UserName"] = user.FullName;
} else {
    ShowErrorAlert("Invalid email or password");
}
```

### 4. UserService.cs

**Location**: `BusinessLogic/UserService.cs`

**Namespace**: `AttireZone_Web_App.BusinessLogic`
**Class**: `UserService` (static)

**Public Methods**:

#### RegisterUser()

```csharp
public static bool RegisterUser(string fullName, string email, string password)
```

- Validates input
- Checks if email exists
- Hashes password
- Inserts user into database
- Returns: true/false

#### AuthenticateUser()

```csharp
public static User AuthenticateUser(string email, string password)
```

- Validates input
- Queries user by email
- Verifies password hash
- Returns: User object or null

#### UserExists()

```csharp
public static bool UserExists(string email)
```

- Checks if email is registered
- Returns: true/false

**Private Methods**:

#### HashPassword()

```csharp
private static string HashPassword(string password)
```

- Uses SHA-256 algorithm
- Returns: Base64 encoded hash

#### VerifyPassword()

```csharp
private static bool VerifyPassword(string password, string hash)
```

- Compares password hash
- Returns: true/false

---

## Usage Examples

### Example 1: Register a New User

```csharp
// In Register.aspx.cs
protected void btnRegister_Click(object sender, EventArgs e)
{
    string fullName = txtFullName.Text.Trim();
    string email = txtEmail.Text.Trim();
    string password = txtPassword.Text.Trim();

    if (UserService.RegisterUser(fullName, email, password))
    {
        Response.Redirect("~/Auth/Login.aspx");
    }
    else
    {
        ShowErrorAlert("Email already exists");
    }
}
```

### Example 2: Authenticate User

```csharp
// In Login.aspx.cs
protected void btnLogin_Click(object sender, EventArgs e)
{
    User user = UserService.AuthenticateUser(
        txtEmail.Text.Trim(),
        txtPassword.Text.Trim()
    );

    if (user != null)
    {
        Session["UserId"] = user.UserId;
        Session["UserName"] = user.FullName;
        Response.Redirect("~/Default.aspx");
    }
    else
    {
        ShowErrorAlert("Invalid credentials");
    }
}
```

### Example 3: Check if User is Logged In

```csharp
// In any page code-behind
if (Session["UserId"] != null)
{
    int userId = (int)Session["UserId"];
    string userName = Session["UserName"].ToString();
    // User is logged in
}
else
{
    // User is not logged in
    Response.Redirect("~/Auth/Login.aspx");
}
```

### Example 4: Create a Logout Page

```csharp
// Logout.aspx.cs
protected void Page_Load(object sender, EventArgs e)
{
    // Clear session
    Session.Clear();
    Session.Abandon();

    // Redirect to home
    Response.Redirect("~/Default.aspx");
}
```

---

## Database Schema

### Users Table

```sql
CREATE TABLE [dbo].[Users] (
    [UserId] INT IDENTITY(1,1) PRIMARY KEY,
    [FullName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL UNIQUE,
    [Password] NVARCHAR(255) NOT NULL,
    [CreatedDate] DATETIME DEFAULT GETDATE(),
    [LastModifiedDate] DATETIME DEFAULT GETDATE()
);

CREATE INDEX [IX_Users_Email] ON [dbo].[Users]([Email]);
```

### Insert Query Example

```sql
INSERT INTO [dbo].[Users]
    ([FullName], [Email], [Password], [CreatedDate], [LastModifiedDate])
VALUES
    ('John Doe', 'john@example.com', 'hashed_password_here', GETDATE(), GETDATE());
```

### Select Query Example

```sql
SELECT UserId, FullName, Email, CreatedDate
FROM [dbo].[Users]
WHERE Email = @Email;
```

---

## SQL Parameterized Requests

All database queries use parameterized queries to prevent SQL injection:

```csharp
string sql = "SELECT * FROM dbo.Users WHERE Email = @Email";
SqlParameter[] parameters = new SqlParameter[]
{
    new SqlParameter("@Email", SqlDbType.NVarChar, 100)
    {
        Value = email
    }
};
DataTable dt = DBHelper.ExecuteDataTable(sql, parameters);
```

---

## CSS Classes

### General Form Classes

```html
<!-- Form group with label and input -->
<div class="form-group">
  <label>Label Text</label>
  <input type="text" class="form-control" />
</div>

<!-- Button styles -->
<button class="btn btn-primary">Primary Button</button>
<button class="btn btn-secondary">Secondary Button</button>

<!-- Alert styles -->
<div class="alert alert-success">Success message</div>
<div class="alert alert-error">Error message</div>
```

### Authentication Page Layout

```html
<div class="auth-container">
  <div class="auth-panel">
    <div class="auth-form-wrapper">
      <div class="auth-card">
        <div class="auth-header">
          <h1>Page Title</h1>
        </div>
        <!-- Form content -->
      </div>
    </div>
  </div>
</div>
```

---

## Extending the System

### Add a New Validation Rule

1. **Client-side** (Register.aspx):

```csharp
<asp:RegularExpressionValidator
    ControlToValidate="txtEmail"
    ValidationExpression="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
    ErrorMessage="Invalid email format" />
```

2. **Server-side** (Register.aspx.cs):

```csharp
if (!email.Contains("@")) {
    ShowErrorAlert("Invalid email format");
    return;
}
```

### Add Remember Me Functionality

```csharp
// In Login.aspx.cs
if (chkRememberMe.Checked)
{
    HttpCookie cookie = new HttpCookie("RememberedEmail", email);
    cookie.Expires = DateTime.Now.AddDays(30);
    Response.Cookies.Add(cookie);
}

// Load remembered email on page load
if (!IsPostBack && Request.Cookies["RememberedEmail"] != null)
{
    txtEmail.Text = Request.Cookies["RememberedEmail"].Value;
}
```

### Add User Profile

1. Create `UserProfile.aspx`
2. Add query to UserService:

```csharp
public static User GetUserById(int userId)
{
    string sql = "SELECT * FROM Users WHERE UserId = @UserId";
    // Execute query and return User object
}
```

### Add Role-Based Access

1. Extend User model with Role property
2. Add role column to database
3. Check role in page_load:

```csharp
if (Session["UserRole"]?.ToString() != "Admin")
{
    Response.Redirect("~/Unauthorized.aspx");
}
```

---

## Security Considerations

### Currently Implemented

✓ Password hashing (SHA-256)
✓ Parameterized SQL queries
✓ Email uniqueness constraint
✓ Session management
✓ Input validation

### Recommended Enhancements

- Use BCrypt or PBKDF2 instead of SHA-256
- Implement HTTPS enforcement
- Add rate limiting on login attempts
- Implement CSRF tokens
- Add logging for security events
- Implement 2-factor authentication
- Add email verification on signup

---

## Common Issues & Solutions

### Issue: User registration succeeds but can't login

**Solution**: Check if password hash is being stored correctly

```csharp
// Debug: Print hash before storing
string hash = HashPassword(password);
Debug.WriteLine("Password Hash: " + hash);
```

### Issue: Session clears too quickly

**Solution**: Check Web.config session timeout

```xml
<sessionState timeout="30" />
```

### Issue: Database connection fails

**Solution**: Verify connection string

```xml
<add name="AttireZone"
     connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=AttireZone;Integrated Security=True;"
     providerName="System.Data.SqlClient" />
```

---

## Performance Tips

1. **Add email index** (already done):

```sql
CREATE INDEX IX_Users_Email ON dbo.Users(Email);
```

2. **Cache user data** (if frequently accessed):

```csharp
Cache["User_" + userId] = user;
```

3. **Use connection pooling** (automatic in SQL Server):

```xml
connectionString="...;Pooling=true;Max Pool Size=100;"
```

---

## Documentation Standards

### Code Comments

```csharp
/// <summary>
/// Brief description of method
/// </summary>
/// <param name="paramName">Parameter description</param>
/// <returns>Return value description</returns>
public static bool MethodName(string paramName)
{
    // Implementation
}
```

### Method Documentation Example

```csharp
/// <summary>
/// Registers a new user in the system with email and password
/// </summary>
/// <param name="fullName">User's full name (required)</param>
/// <param name="email">User's email (must be unique)</param>
/// <param name="password">User's password (will be hashed)</param>
/// <returns>
/// True if registration successful; false if email exists or validation fails
/// </returns>
public static bool RegisterUser(string fullName, string email, string password)
```

---

## Testing Guidelines

### Unit Test Example (Pseudocode)

```csharp
[TestMethod]
public void RegisterUser_ValidInput_ReturnsTrue()
{
    // Arrange
    string fullName = "Test User";
    string email = "test@example.com";
    string password = "TestPassword123";

    // Act
    bool result = UserService.RegisterUser(fullName, email, password);

    // Assert
    Assert.IsTrue(result);
}

[TestMethod]
public void RegisterUser_DuplicateEmail_ReturnsFalse()
{
    // Arrange
    UserService.RegisterUser("User1", "duplicate@test.com", "Pass123");

    // Act
    bool result = UserService.RegisterUser("User2", "duplicate@test.com", "Pass456");

    // Assert
    Assert.IsFalse(result);
}
```

---

## Version History

- **v1.0** (Current)
  - Initial implementation
  - Register page with form validation
  - Login page with authentication
  - Password hashing with SHA-256
  - Session management
  - Remember me functionality
  - Professional design system

---

**Last Updated**: March 26, 2026
**Maintained By**: Development Team
**Next Review**: 2 months

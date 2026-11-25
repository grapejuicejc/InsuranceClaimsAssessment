# Insurance Claims

A claims application for motor and property insurance. Claimants report an incident and track it. Claims staff pick up
claims and settle or reject them.

- **Backend:** ASP.NET Core Web API (.NET 10), Entity Framework Core 10, SQL Server LocalDB, JWT login
- **Frontend:** Angular 22

> **In short:** a claim can go from report to decision, through both the API and the Angular app. Kafka, information
> requests, the team dashboard and outstanding liability were **not built**. See
> [What was missed](#what-was-missed-because-of-time).

## Contents

1. [What you need](#1-what-you-need)
2. [Get the code](#2-get-the-code)
3. [Set up the backend](#3-set-up-the-backend)
4. [Start the backend](#4-start-the-backend)
5. [Start the frontend](#5-start-the-frontend)
6. [Log in](#6-log-in)
7. [A guided tour](#7-a-guided-tour)
8. [Problems and fixes](#8-problems-and-fixes)
9. [What was added](#what-was-added)
10. [What was missed because of time](#what-was-missed-because-of-time)
11. [How it is designed](#how-it-is-designed)

Setting up takes about 10 to 15 minutes, mostly downloads.

## 1. What you need

| Tool | Version | Check with |
|---|---|---|
| Windows | 10 or 11 (developed on 11) | |
| .NET SDK | 10 | `dotnet --version` |
| SQL Server LocalDB | 2019 was used | `SqlLocalDB info` |
| Node.js | 24.15 or later (24.18 was used) | `node --version` |
| Git | Any | `git --version` |

- **LocalDB** is included with Visual Studio's *ASP.NET and web development* or *Data storage and processing* workload,
  and with SQL Server Express. `SqlLocalDB info` should list `MSSQLLocalDB`.
- You do **not** need Docker, Kafka, Visual Studio or a global Angular CLI. Visual Studio instructions are given as an
  option.
- **LocalDB only works on Windows.** To use another SQL Server, see [Problems and fixes](#8-problems-and-fixes). This
  has not been tested.

All commands below are for **PowerShell**.

## 2. Get the code

The repository is private, so you need to have been given access.

```powershell
git clone https://github.com/grapejuicejc/InsuranceClaimsAssessment.git
cd InsuranceClaimsAssessment
```

If you were sent a ZIP file, extract it and open PowerShell in the folder that contains `InsuranceClaimsAssessment.slnx`.
This README calls it the **root folder**.

| Folder | Contents |
|---|---|
| `InsuranceClaimsAssessment/` | Backend API |
| `claims-web/` | Angular frontend |

## 3. Set up the backend

Do this once. From the root folder:

```powershell
cd InsuranceClaimsAssessment
```

You should now be in the folder that contains `InsuranceClaimsAssessment.csproj`.

### 3.1 Trust the development certificate

```powershell
dotnet dev-certs https --trust
```

Choose **Yes** if Windows asks. The API uses HTTPS locally.

### 3.2 Create a signing key

The key signs login tokens. It is kept in .NET user secrets, outside the repository, so each machine needs its own.

```powershell
$b = New-Object byte[] 32
[Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($b)
dotnet user-secrets set "Jwt:Key" ([Convert]::ToBase64String($b))
```

**Do this before the next step.** Without the key, the database commands and the API stop with an error.

### 3.3 Create the database

This creates a LocalDB database named `InsuranceClaimsAssessment`. Use **one** of these options.

**Option A: command line**

```powershell
dotnet tool install --global dotnet-ef --version 10.0.15
dotnet ef database update
```

If `dotnet-ef` is already installed, skip the first line. If `dotnet ef` is not found, open a new PowerShell window in
the same folder and try again.

**Option B: Visual Studio**

1. Open `InsuranceClaimsAssessment.slnx`.
2. Choose **Tools > NuGet Package Manager > Package Manager Console**.
3. Check that **Default project** is `InsuranceClaimsAssessment`.
4. Run `Update-Database`.

The tables are created empty. Demo users and policies are added the first time the API starts.

## 4. Start the backend

**Option A: command line**, in the folder with `InsuranceClaimsAssessment.csproj`:

```powershell
dotnet run --launch-profile https
```

**Option B: Visual Studio.** Choose the **https** profile next to the start button and press **Ctrl+F5**.

Wait for this, and **leave the window open**:

```
Now listening on: https://localhost:7140
Now listening on: http://localhost:5155
```

No browser opens, and `https://localhost:7140/` shows a 404. That is expected, because the backend is only an API. To
check it works, open <https://localhost:7140/openapi/v1.json>. It should show a list of endpoints as JSON.

## 5. Start the frontend

Open a **second** PowerShell window in the root folder:

```powershell
cd claims-web
npm ci
npm start
```

`npm ci` downloads the packages and is needed only once. Warnings starting with `npm warn allow-scripts` can be
ignored. If Angular asks about sharing usage data, either answer works.

Wait for `Local: http://localhost:4200/`, leave the window open, and open <http://localhost:4200> in a browser.

**Both windows must stay open.** The Angular development server passes `/api` calls to the backend at
`https://localhost:7140`.

## 6. Log in

The password for every demo account is `Password123!`.

| Email | Role | Policies |
|---|---|---|
| `claimant1@example.com` | Claimant | `MTR-0001` (motor), `PRP-0001` (property) |
| `claimant2@example.com` | Claimant | `MTR-0002` (motor) |
| `staff1@example.com` | Claims staff | |
| `staff2@example.com` | Claims staff | |

These are made-up accounts for local use only. They are added in Development, and only when there are no users.

**Tip:** use one normal and one private browser window to be logged in as a claimant and as staff at the same time.

## 7. A guided tour

### Follow one claim from report to decision

1. **Report it.** Log in as `claimant1@example.com` and choose **Report an incident**. Pick a policy, enter a recent
   date and a description, and choose **Report incident**. The new claim opens with status **Submitted**.
2. **Pick it up.** In the private window, log in as `staff1@example.com`. The claim is under **Incoming**. Choose
   **Pick up**.
3. **See the change.** In the claimant's window, refresh the page. The status is now **Under review**.
4. **Decide it.** As staff, open **My workload** and choose **Decide**. Enter a reason, and an amount to settle. Choose
   **Settle** or **Reject**, then confirm.
5. **Receive the decision.** Refresh the claimant's page. It shows the decision, the reason, the amount and the history.

Pages do not refresh by themselves.

### Try the rules

| Try this | Expected |
|---|---|
| Report an incident with a future date | "The incident date cannot be in the future." |
| Open a claim's address, such as `/claims/1`, as the other claimant | "Claim not found." |
| Log in as `staff2`, then pick up in an old list a claim that `staff1` has just taken | "This claim has already been picked up." |
| Settle without an amount | "Enter an amount to settle." |
| Log in with a wrong password | "Invalid email or password." |

### Look at the API

| Method | Address | Who |
|---|---|---|
| `POST` | `/api/auth/login` | Anyone |
| `GET` | `/api/policies` | Claimant: own policies |
| `POST` | `/api/claims` | Claimant: report an incident |
| `GET` | `/api/claims` | Claimant: own claims |
| `GET` | `/api/claims/{id}` | Claimant: own claim, with history and decision |
| `GET` | `/api/staff/claims?status=Submitted&mine=true` | Staff: both filters are optional |
| `POST` | `/api/claims/{id}/pickup` | Staff |
| `POST` | `/api/claims/{id}/settle` | Assigned staff member |
| `POST` | `/api/claims/{id}/reject` | Assigned staff member |

`InsuranceClaimsAssessment/InsuranceClaimsAssessment.http` has sample requests for Visual Studio. The template's
`/weatherforecast` sample is still there and is not part of the solution.

### Where to look in the code

| To see | Open |
|---|---|
| Rules for reporting and tracking | `InsuranceClaimsAssessment/Services/ClaimService.cs` |
| Rules for pickup and decisions | `InsuranceClaimsAssessment/Services/StaffClaimService.cs` |
| Database mapping | `InsuranceClaimsAssessment/Data/ClaimsDbContext.cs` |
| The frontend/backend contract | `InsuranceClaimsAssessment/Contracts/` |
| Angular login and token handling | `claims-web/src/app/auth/` |
| Claimant pages | `claims-web/src/app/claims/` |
| Staff pages | `claims-web/src/app/staff/` |

### Run the test

```powershell
cd claims-web
npx ng test --watch=false
```

There is one small frontend test. There are no backend tests.

## 8. Problems and fixes

| Problem | Fix |
|---|---|
| `Jwt:Key is missing` | Do step 3.2 in the folder with the `.csproj` file |
| `Unable to create a 'DbContext'` from `dotnet ef` | Usually the same cause: do step 3.2 first |
| `Cannot open database "InsuranceClaimsAssessment"` or `Invalid object name 'Users'` | Do step 3.3 |
| LocalDB cannot be found or started | Run `SqlLocalDB start MSSQLLocalDB`. If it is not installed, see step 1 |
| A port is in use | Close the other program. The backend needs 7140, and Angular uses 4200 |
| Backend shows only port 5155 | Start it with the **https** profile |
| Login says "Could not log in" | Check the backend window is open and listening on 7140 |
| Browser or PowerShell certificate warning | Do step 3.1, then restart the browser |
| PowerShell says running scripts is disabled | Use `npm.cmd` and `npx.cmd` instead of `npm` and `npx` |
| Suddenly returned to the login page | Tokens last 60 minutes. Log in again |
| Demo accounts are missing | Use the **https** profile, which runs in Development. Accounts are added only when there are no users |

**To use another SQL Server**, set this in PowerShell before steps 3.3 and 4 (not tested):

```powershell
$env:ConnectionStrings__ClaimsDb = "Server=YOUR_SERVER;Database=InsuranceClaimsAssessment;Trusted_Connection=True;TrustServerCertificate=True"
```

## What was added

| Area | Added |
|---|---|
| **Claimants** | Report an incident, see own claims, see one claim with its history and decision |
| **Claims staff** | Incoming claims, own workload, all claims, pick up, settle, reject |
| **Login** | Email and password, hashed passwords, 60-minute JWT, role checks on every claim endpoint |
| **Data** | Seven tables through an EF Core migration, demo users and policies |
| **Safety** | Two staff members cannot pick up the same claim; a claim cannot be decided twice; claimants see only their own claims and policies |
| **Frontend** | One Angular app for both roles: login, claimant pages, staff pages |

### What was tested

There is no automated backend test suite. Checks were run by hand and with temporary scripts that are not in the
repository.

| Tested on | What |
|---|---|
| SQL Server LocalDB | Migration, seed data, login, reporting through the API, tracking, pickup |
| SQLite, in a temporary copy | Settle, reject, 10 pickups at once, 10 decisions at once, the report page, the staff pages |

Settling, rejecting and simultaneous requests have **not** been run on SQL Server.

## What was missed because of time

The exercise was time-limited. These parts are **not built**:

| Missed | What exists today | What I would do next |
|---|---|---|
| **Kafka and events** | Nothing is published. The design is below | Build the in-memory version, then Kafka, then an outbox |
| **Information requests** | Table, and display on the claim page. Staff cannot ask and claimants cannot reply | Two endpoints and two small forms |
| **Team workload and performance** | Staff see lists only | Counts per staff member, and time from report to decision |
| **Outstanding liability** | Estimated and settled amounts are stored. Nothing adds them up | Agree a definition, then total the open claims |
| **Attachments** | Table only. Files cannot be uploaded or downloaded | Upload endpoint with a size limit and type checks |
| **Backend tests** | None | Tests for the rules in the two services, including simultaneous requests |
| **Staff claim details** | The staff list includes the description, but not the history | A details endpoint and page |
| **Automatic updates** | Pages must be refreshed | Push changes to the browser, driven by the events |

### Simplified by choice

| Choice | Effect |
|---|---|
| No markets, and one unnamed currency | Claims and liability cannot be split by market or currency |
| Settling is the decision | Approved-but-unpaid money is not tracked |
| No manager role | Managers have no view of their own |
| No reassignment | Claims of an absent staff member get stuck |
| No withdrawal, reopening or appeals | `Settled` and `Rejected` are final |
| No registration or password reset | Only the demo accounts exist |

### Known limitations

- A login for an unknown email is answered faster than one with a wrong password, and login attempts are not limited.
- A token cannot be cancelled before it expires.
- An estimated amount with three decimals is accepted, but the database keeps two.
- A description of only spaces is refused with a general message.
- The TypeScript types are copied by hand from the API's `Contracts` classes.
- The Angular proxy is for development only. A deployed version would need a reverse proxy or CORS.

## How it is designed

### Claim lifecycle

```
Submitted  ->  UnderReview  ->  Settled
                            ->  Rejected
```

Any staff member can pick up a `Submitted` claim. After that, only the assigned staff member can decide it. Settling
needs an amount and a reason; rejecting needs a reason.

"Waiting for the claimant" is not a status. It is worked out from an unanswered information request.

### Entities

```
User   1 --- *  Policy
User   1 --- *  Claim                (the claimant)
User   1 --- *  Claim                (the assigned staff member, empty until pickup)
Policy 1 --- *  Claim
Claim  1 --- *  ClaimStatusHistory
Claim  1 --- *  InformationRequest
Claim  1 --- *  Attachment
Claim  1 --- 0..1  ClaimDecision
InformationRequest  1 --- *  Attachment   (optional)
```

### One service

It is one backend service with one database. Saving a claim's status, history and decision together needs a single
transaction, and splitting it would add work without a benefit at this size.

### REST now, events later

REST is used for actions, because the user needs the answer immediately. Kafka was planned for announcing what happened,
so that work which can wait does not slow the user down. **This part is design only:**

- Events: `ClaimSubmitted`, `ClaimPickedUp`, `ClaimSettled`, `ClaimRejected`
- One topic, keyed by claim ID, so each claim's events stay in order
- An interface with in-memory and Kafka versions, chosen by a setting, so the app starts without Kafka
- Published after the save. An outbox table would stop events being lost while Kafka is down

### Simultaneous changes

`Claim` has a SQL Server `rowversion`. If someone else saved the claim first, the second save is refused and the API
answers 409. A unique index also allows only one decision per claim.

### Security

- The user's ID and role come from the signed token, never from the request.
- Someone else's claim or policy gives 404, so nobody learns what exists.
- Entities are never returned, so a password hash cannot leak.
- Angular's guards are only a convenience. The API makes the real checks.

## AI use

This project was built with an AI assistant. [AI_JOURNAL.md](AI_JOURNAL.md) records what I asked, accepted, challenged
and overrode.

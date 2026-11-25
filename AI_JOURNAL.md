# AI working journal


## How I worked with it

I asked for one small step at a time. The assistant had to explain a choice, let me decide, and then write only that
piece. It was not allowed to commit, push or create branches unless I asked, or to overwrite my edits.

For most steps it gave me options with a recommendation, and I accepted, changed or refused them. It usually tested its
code in a temporary copy before I tried it. I kept Visual Studio for the backend and VS Code for the Angular app.

## Deciding the claim lifecycle

I started with a decision, not with code. My first prompt was:

> Read CLAUDE.md. Help me decide the claim lifecycle. Explain the options first and don't write code yet.

The assistant offered three options and recommended five statuses, with "information requested" as a status of its own.
I overrode it and chose four: Submitted, UnderReview, Settled and Rejected. I also challenged its wording, because it
kept writing about an "assigned officer":

> Option A. Only Claimant and Claims Staff. We dont need the role of officer.

The effect is that "waiting for the claimant" is worked out from an unanswered request instead of being stored. Every
screen that shows it has to make that check. I accepted the other rules: only the staff member who picked up a claim can
decide it, decisions are final, and reassignment, withdrawal, reopening and appeals are left out.

## Designing the entities

This is where I overrode the assistant most. It suggested a small model: a user with only an ID, a name and a role, a
policy number stored as text, and the decision and dates as fields on the claim. It also suggested leaving attachments
out. I gave it my own lists of properties instead, ending each one with:

> i want these

My model has a separate Policy, ClaimDecision, ClaimStatusHistory and Attachment. The assistant wrote them as I listed
them and then pointed out what was missing against my own rules. There was no assigned staff member, no settled amount
and no estimated amount. I accepted those additions.

It also added a Market table, which I had first agreed to. I changed my mind:

> we dont need market.cs

It warned me that the exercise covers six markets and that totals in different currencies cannot be added together. It
recommended storing a market and a currency code on the policy. I chose to leave markets out, which means claims and
liability cannot be split by market or currency. That is listed as a limitation in the README. I also paused the next
step until I was ready:

> option c, pause on dbcontext

## Writing the database mapping myself

I added my own OnModelCreating code and asked for a review:

> i added some codes in my ClaimsDbContext.cs. what do you think

The assistant could not see my changes, because I had not saved the file. It said so instead of guessing, and I pasted
the code. Its review found that my mapping fixed the cascade delete problem, and it suggested four changes. I committed
my own version first:

> saved, dont make any changes, commit without co-author and push with message: Added DBContext file.

I then accepted all four changes: a row version so that two staff members cannot pick up the same claim, no cascade
deletes, timestamps marked as UTC, and the mapping kept in one place.

I ran the migration commands myself and asked for a check before and after:

> migration created, review it

> database updated, check the tables

## Login and seed data

I chose login with a signed token. I accepted the password hashing, the key kept in user secrets and the 60-minute
token, but not the seed data:

> A, yes to 1, 2 and 3 only

The assistant pointed out that nobody could log in to an empty database. I changed my mind:

> write seed data first

I set the signing key myself, so it never appeared in the conversation. When the app ran without showing a page, I asked:

> is the app supposed to open on web?

The answer was no, because the backend is only an API. I also asked for a summary to see where I stood:

> what api has been done?

## Reporting a claim, and a test that failed

I accepted the assistant's recommendations for reporting an incident: a random reference number, checks on the policy
and the incident date, and the rules in a service. The claimant's ID comes from the token and never from the request.

When I tried it, the PowerShell command the assistant gave me failed with a 400 error. It reproduced the likely cause in
a temporary copy: the long line had been wrapped when I copied it, which broke the JSON. It gave me a shorter version,
which worked, and it confirmed the saved rows in the database.

## Starting Angular before Kafka

The assistant advised me more than once to finish the staff endpoints and Kafka first. I decided otherwise:

> lets do angular now. I want to do it on vs code

The Angular app was first created inside the backend folder. The assistant found this when it checked the generated
files, and I moved the app to the root. I then asked for the pages one part at a time:

> yes to all, and make the front end for the claim staff

> let claimant make a claim

The staff endpoints were tested with ten pickups at once and ten decisions at once. Each time, one request succeeded and
nine were refused. The decision test also showed that the unique index, not the row version, refused the duplicates, so
both had to be handled to avoid a server error.

I accepted a design for the events: REST for actions, and Kafka to announce what happened. I did not build it, and I
decided to stop:

> works. I think we can stop here. After the last push. Come back for my instructions on read me

## Controlling the commits

I decided what was committed and when. The assistant added a co-author line to its first commit. I asked for that commit
to be removed from GitHub:

> can you revert the commit in remote

It asked me how, because there was more than one way, and I chose to remove it from the branch. I then asked for it to be
committed again without that line, and repeated that for the later commits.

## Documentation

I asked for the README with these instructions:

> write the README and draft the journal too. In the README, i want detailed instructions on how the recruiter can load
> and view my project. In detail. Highlight what was added and missed out due to time restrictions

The first draft of this journal was mostly tables, so I asked for it again:

> i think we can add more the the journal? seems a bit short? can u put it in just sentences. I only can see in VSCode.
> Make it readable for the recruiter. Show some of my prompts

## Mistakes and limits

The assistant's first table design would have been refused by SQL Server, because a claim could be deleted through more
than one path. It found this by checking the generated SQL before any migration existed. It also left an old error
message on the decision form, which a browser test showed. It measured that a login for an unknown email is answered
faster than one with a wrong password, and that is kept as a known limitation.

I ran the claim request through PowerShell and picked up a claim in the app, and both were confirmed in SQL Server.
Settling, rejecting, simultaneous requests and the report page were tested by the assistant on temporary SQLite
databases. They have not been confirmed on SQL Server. There is no automated backend test suite.



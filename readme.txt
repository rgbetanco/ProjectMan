Projectman Readme
rgbetanco
This webapp is used to let users submit repair requests, and a service engineer 
will use it to report on the status of the repairs.  There are two portals,
one for regular users, and one for admin/service engineers.. each with its own
user/password table.

Repairman Project:
-----------------------------------------------------
https://.../
- Regular User Portal
- Project files are under the main repairman project.
- Users are stored in the "Member" table
- Uses LDAP (settings are in appsettings.json/LDAP/Member) -- remove this "Member" section from settings to disable LDAP.

https://.../man 
- Admin/Service Engineer Portal
- Project files are under the 'man' Area in the repairman project.
- Users are stored in the "User" table
- Default admin/password will be autocreated if addmin account doesn't exist (based on settings in appsetting)
- Uses LDAP (settings are in appsettings.json/LDAP/Admin) -- remove this "Admin" section from settings to disable LDAP.
- Users with "Edit" permission can make changes to existing user submissions.

Folders created by the web app
AppData/temp			- temp images are saved here (not used)
AppData/uploads/request - images submitted by users are uploaded here
AppData/uploads/reply   - images submitted by service engineers are uploaded here
webroot/f/p				- files uploaded by user and service engineers are saved here (for download)
webroot/img/a			- resized user uploaded images are saved here (for download)
webroot/img/a2			- resized service engineer uploaded images are saved here (for download)

CSHelper.net Project
-------------------------------------------------------
- Library containing helper functions used in multiple projects.


LDAP Notes
-------------------------------------------------------
When "AutoCreateAccount" is set to true, when user tries to login
for the first time, the web app will automatically add the user account to
the local database with an empty password --  users that aren't in the 
local database cannot login even if they have a valid LDAP account.

Users must have a non-empty password, whether using LDAP or local database.

Users with enabled accounts (and valid password) in the local database can log in, even if
LDAP is enabled.

Known Issues
-------------------------------------------------------
- Web App might crash if Category / Subcategory / Department tables are empty (not sure if fixed.)

Visual Studio Templates (Project and Item), as well as Code Snippets are going to be packaged in the single csla.vsi file located in the VSI subfolder.

For each of the 3 above mentioned groups we have a specific folder:
1) Item Templates
2) Project Templates
3) Code Snippets

Each of these folders will break down further by .Net language (cs and vb), and then each of the language specific folders will have 2 sub-folders:
- Templates Source
- Generated Templates

For both Item Templates and Project Templates, "Templates Source" contains one subfolder for each template (eg. CommandObject, ReadOnlyList,...)  Each of these subfolders contains the content files for the templates that will be generated.

The process of generating the template (once the content files are edited), is to package content files in a zip file with the same name as the folder containing templates (CommandObject folder content goes to CommandObject.zip).

There will be an automated tool - batch script that will do the task of zipping of all these folders automatically.  The script can be found at Visual Studio Templates\Buil folder.

Once the tool creates the individual Item Template zip files (which can be manually deployed to the Visual Studio 2008\Templates\Item Templates\Visual C#\CSLA.NET folder), then the next task of the build tool is to package these item templates and project templates zip files into another zip file called csla.zip.  The build tool will also add the csla.vscontent xml file which is the file that declares all of the templates that should be installed as a part of that package.

Once all that is placed in the csla.zip (in VSI subfolder), then the last step is to rename the csla.zip to csla.vsi.


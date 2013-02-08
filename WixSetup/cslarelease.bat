rd /q/s Release
md Release
cd Release

svn export svn://svn.lhotka.net/csla/core/trunk/Source Source
svn export svn://svn.lhotka.net/csla/core/trunk/Support Support

svn export svn://svn.lhotka.net/csla/core/trunk/license.txt
svn export svn://svn.lhotka.net/csla/core/trunk/readme.txt

zip -r ..\CslaSource *

cd ..
rd /q/s Release
md Release
cd Release



svn export svn://svn.lhotka.net/csla/core/trunk/Samples Samples

svn export svn://svn.lhotka.net/csla/core/trunk/license.txt
svn export svn://svn.lhotka.net/csla/core/trunk/readme.txt

zip -r ..\CslaSamples *

cd ..
rd /q/s Release

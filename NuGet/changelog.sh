git log $1..HEAD --no-merges --format='%cn,%cE,%s' | awk '
BEGIN {
    FS=","
}
{
    n=split($2,array,"@");
    email=array[1];
    n=split($3,array," ");
    title="";
    for (i=1;i<=n;i++)
        if (substr(array[i],1,1) == "#") {
            title = title "[" array[i] "](https://github.com/marimerllc/csla/issues/" substr(array[i], 2) ") " 
        } else {
            title = title array[i] " ";
        }
    print "* " title;
}'
# git log v4.5.701..HEAD --no-merges --format='{"author":"%an", "email":"%cE", "title":"%s"}'
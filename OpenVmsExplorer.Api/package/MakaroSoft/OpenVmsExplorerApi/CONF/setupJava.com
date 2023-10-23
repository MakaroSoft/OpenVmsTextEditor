$ @sys$startup:openjdk$setup.com
$
$! used to be 8 which worked for java$80 but not openjdk
$ define/job/nolog java$filename_controls 0
$
$ define/job java$enable_environment_expansion TRUE

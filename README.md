## Simple set of scripts for analysing Git repos

Check out the repo and run "run.bat" with the path to a git repository as a parameter. You should end up with a "glue.html" file in output with some very basic stats regarding that repo.

### It can't find git!

I've defaulted the git bin path to: ``@"C:\Program Files (x86)\Git\bin\git.exe"``. You may need to override this as an environment variable (``git``) or just change it in the ``src/readLog.fsx`` file

### I want to analysis different things!

That's kind of the whole point :). Crack open ``generate/glue.fsx`` and start adding things. The data manipulation is being done by [Deedle](http://bluemountaincapital.github.io/Deedle) which I'm enjoying a lot, but I'm not an expert in by any means.

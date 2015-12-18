================================================================================
==                            React.js Starter Kit                            ==
================================================================================

This is a cross-platform project template built on top of the Facebook's React.js
library and powered by Node.js-based development tools and utilities (it requires
Node.js only at development time, you don't need to have it installed on your web
server, unless you want to). It contains only front-end part of a web application
and is recommended to be paired with a server-side project (REST API, Web Sockets)

Source code: https://github.com/kriasoft/react-starter-kit

PREREQUISITES
--------------------------------------------------------------------------------

- Node.js
- NPM (comes with Node.js)
- Gulp (to install run: npm install -g gulp)

HOW TO RESTORE NPM PACKAGES
--------------------------------------------------------------------------------

Open a console window, change the working directory to a folder where project
files are located, then run:

> npm install

Note: If you see error messages during install, something went wrong, check this
      page https://github.com/TooTallNate/node-gyp/ The packages listed in the
      package.json file are installed into the 'node_modules' folder. You can
      delete this folder if needed by running: cmd /c "rmdir /s /q node_modules"

HOW TO BUILD
--------------------------------------------------------------------------------

> gulp build                  # Builds the project in debug mode (default)
> gulp build --release        # Builds the project in release mode

Note: The build output files are written into the .\build folder.
      The build logic is described in gulpfile.js.

HOW TO BUILD AND RUN
--------------------------------------------------------------------------------

> gulp                        # Build the project and open it in a browser
> gulp --release              # Same as above, but using release mode

Note: It will build the project, run a development HTTP server (BrowserSync),
      open the home page of your web application in a browser window and start
      watching for modifications in files (LiveReload).

HOW TO DEPLOY
--------------------------------------------------------------------------------

> gulp deploy                 # Deploys the app from the .\build folder
> gulp deploy --production    # Same as above, but using a different destination


SUPPORT
--------------------------------------------------------------------------------

Have questions, feedback or need help?
Contact me on https://www.codementor.io/koistya

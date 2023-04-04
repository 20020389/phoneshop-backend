const path = require("path");
const mysql = require("mysql");
const { exec } = require("child_process");
const consola = require("consola");
const settings = require("../appsettings.json");
const { readFile } = require("fs/promises");

const task = process.argv[2];
const paramater = process.argv[3];
(async function main() {
  switch (task) {
    case "--build":
      const {
        Username: username,
        Password: password,
        Host: host,
        DbName: dbName,
        Port: port,
      } = settings.Prisma;

      const databaseUrl = `mysql://${username}:${password}@${host}:${port}/${dbName}`;
      process.env.DATABASE_URL = databaseUrl;
      /**@type {import('mysql').Connection} */
      let connection;
      if (paramater === "--clear") {
        try {
          connection = await clearDatabase({
            host,
            port,
            user: username,
            password,
            database: dbName,
          });
          await runScript("yarn clean");
        } catch (e) {
          console.error(e);
          return;
        }
      }

      let buildtype = process.env.NODE_ENV == "production" ? "pro" : "dev";

      let script = `yarn build:${buildtype} --schema ./database.prisma`;
      await runScript(script);
      if (paramater === "--clear") {
        if (connection) {
          await addTrigger(connection, dbName);
        }
      }
      script = `cd .. && dotnet ef dbcontext scaffold "Host=${host};Database=${dbName};Username=${username};Password=${password};" MySql.EntityFrameworkCore --context-dir Prisma --context PrismaClient --output-dir Model -f --no-build`;
      await runScript(script);

      if (connection) {
        connection.end();
      }
      break;
    default:
      throw "No command to excute";
  }
})().catch((error) => {
  console.log(error);
});

function runScript(script) {
  return new Promise((resolve, reject) => {
    consola.info({
      message: `${script}`,
      badge: true,
    });
    exec(script, (error, stdout, stderr) => {
      if (error) {
        consola.error({
          message: `error: ${error.message}`,
          badge: true,
        });
        reject(stderr);
      }
      if (stderr) {
        consola.error({
          message: `stderr: ${stderr}`,
          badge: true,
        });
        reject(stderr);
      }
      consola.success({
        message: `stdout: ${stdout}`,
        badge: true,
      });
      resolve("");
    });
  });
}

function clearDatabase({ host, port, user, password, database }) {
  return new Promise(async (resolve, reject) => {
    const connection = await connectDatabase({
      host,
      port,
      user,
      password,
    });
    // @ts-ignore
    connection.query(
      `DROP DATABASE IF EXISTS ${database}`,
      function (err, result) {
        // @ts-ignore
        connection.destroy();
        if (err) reject(err);
        consola.success({
          message: "Droped Database",
          badge: true,
        });
        resolve("");
      }
    );

    return connection;
  });
}

/**
 *
 * @param {string} databaseUrl
 * @returns {Promise<mysql.Connection>}
 */
function connectDatabase({ host, port, user, password, database }) {
  return new Promise((resolve, reject) => {
    const connection = mysql.createConnection({
      host,
      port,
      user,
      password,
      database,
    });

    connection.connect(function (err) {
      if (err) reject(err);
      resolve(connection);
    });

    return connection;
  });
}

async function addTrigger(connection, database) {
  const content = await readFile("./trigger.sql", "utf-8");
  if (content) {
    content = content.replace("$DATABASE$", database);
    connection.query(content, function (err, result) {
      // @ts-ignore
      connection.destroy();
      if (err) reject(err);
      consola.success({
        message: "Droped Database",
        badge: true,
      });
      resolve("");
    });
  }
}

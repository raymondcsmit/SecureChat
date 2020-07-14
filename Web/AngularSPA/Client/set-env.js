const fs = require('fs');
const yargs = require('yargs');

const environment = yargs.argv.environment;
const isProduction = yargs.argv.environment === "prod";
const targetPath = environment ? `./src/environments/environment.${environment}.ts` : './src/environments/environment.ts';

const envFile = `export const environment = {
  production: ${isProduction},
  authorityUrl: "${process.env.AuthUrl}",
  clientId: "${process.env.ClientId}",
  accountApi: "${process.env.AccountApiUrl}",
  usersApi: "${process.env.UsersApiUrl}",
  sessionApi: "${process.env.SessionApiUrl}",
  messagingUrl: "${process.env.MessagingUrl}",
  chatsApi: "${process.env.ChatsApiUrl}"
}`

fs.writeFileSync(targetPath, envFile);
console.log(`New ${targetPath} file: ${envFile}`);
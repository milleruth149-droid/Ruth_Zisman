import { randomBytes, createHmac } from 'node:crypto';
import { execFileSync } from 'node:child_process';
import { fileURLToPath } from 'node:url';
import { dirname, resolve } from 'node:path';

const projectPath = resolve(
  dirname(fileURLToPath(import.meta.url)),
  '..',
  'MuniServices.Api.csproj'
);

function setSecret(name, value) {
  try {
    execFileSync(
      'dotnet',
      ['user-secrets', 'set', name, value, '--project', projectPath],
      { stdio: 'ignore' }
    );
  } catch {
    throw new Error(`Could not save ${name} to .NET User Secrets.`);
  }
}

function readSecrets() {
  const output = execFileSync(
    'dotnet',
    ['user-secrets', 'list', '--project', projectPath],
    { encoding: 'utf8' }
  );
  const secrets = new Map();

  for (const line of output.split(/\r?\n/)) {
    const separator = line.indexOf(' = ');
    if (separator < 0) {
      continue;
    }

    secrets.set(line.slice(0, separator), line.slice(separator + 3));
  }

  return secrets;
}

try {
  let secrets = readSecrets();
  const devIssuer = 'MuniServices.LocalDevelopment';
  const devAudience = 'MuniServices.LocalDevelopment';
  const configuredIssuer = secrets.get('Authentication:Issuer');
  const configuredAudience = secrets.get('Authentication:Audience');

  if (configuredIssuer && configuredIssuer !== devIssuer) {
    throw new Error(
      'Configured issuer is not the local-development issuer; refusing to use it for a test token.'
    );
  }
  if (configuredAudience && configuredAudience !== devAudience) {
    throw new Error(
      'Configured audience is not the local-development audience; refusing to use it for a test token.'
    );
  }


  // מוודא שההגדרות מיועדות לפיתוח מקומי,

  const hasDevSettings =
    configuredIssuer === devIssuer &&
    configuredAudience === devAudience &&
    secrets.has('Authentication:SigningKey');

  if (!hasDevSettings) {
    setSecret('Authentication:Issuer', devIssuer);
    setSecret('Authentication:Audience', devAudience);
    setSecret(
      'Authentication:SigningKey',
      randomBytes(32).toString('base64')
    );
  }

  secrets = readSecrets();

  const issuer = secrets.get('Authentication:Issuer');
  const audience = secrets.get('Authentication:Audience');
  const signingKeyValue = secrets.get('Authentication:SigningKey');

  if (!issuer || !audience || !signingKeyValue) {
    throw new Error('JWT settings could not be read from .NET User Secrets.');
  }

  const signingKey = Buffer.from(signingKeyValue, 'base64');
  if (signingKey.length < 32) {
    throw new Error('Authentication:SigningKey must contain at least 32 bytes.');
  }

  const now = Math.floor(Date.now() / 1000);
  const encode = (value) => Buffer.from(JSON.stringify(value)).toString('base64url');
  const header = encode({ alg: 'HS256', typ: 'JWT' });
  const payload = encode({
    iss: issuer,
    aud: audience,
    sub: 'local-development-test-user',
    idNumber: '012345678',
    iat: now,

    // טוקן הבדיקה תקף לשעה אחת בלבד.
    exp: now + 60 * 60
  });
  const unsignedToken = `${header}.${payload}`;
  const signature = createHmac('sha256', signingKey)
    .update(unsignedToken)
    .digest('base64url');

  console.log(`${unsignedToken}.${signature}`);
  console.error(
    'Local development token for mock applicant 012345678; valid for one hour.'
  );
} catch (error) {
  console.error('Could not create the local development JWT.');
  console.error(error instanceof Error ? error.message : error);
  process.exitCode = 1;
}

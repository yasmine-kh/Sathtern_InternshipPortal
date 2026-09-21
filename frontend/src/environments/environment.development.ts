/**
 * Development environment.
 * The port comes from backend/Properties/launchSettings.json ("http" profile,
 * applicationUrl http://localhost:5047) and matches the CORS origin the API
 * allows (http://localhost:4200).
 */
export const environment = {
  production: false,
  /** Backend API base URL for local development. */
  apiBaseUrl: 'http://localhost:5047/api',
};

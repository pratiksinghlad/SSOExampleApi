import React, { useEffect } from "react";
import { MsalProvider } from "@azure/msal-react";
import { authService } from "./services/authService";
import AuthGuard from "./components/AuthGuard";
import HomePage from "./components/HomePage";

const App: React.FC = () => {
  useEffect(() => {
    // Initialize MSAL instance
    authService.initialize().catch(console.error);
  }, []);

  return (
    <MsalProvider instance={authService.getInstance()}>
      <div className="App">
        <AuthGuard>
          <HomePage />
        </AuthGuard>
      </div>
    </MsalProvider>
  );
};

export default App;

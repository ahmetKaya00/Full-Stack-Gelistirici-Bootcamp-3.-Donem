import React, { StrictMode } from 'react'

import ReactDOM from "react-dom/client";
import './index.css'
import App from './App.tsx'
import { QueryClient,QueryClientProvider } from '@tanstack/react-query'
import { AuthProvider } from './auth/AuthContext.tsx'

const queryClient = new QueryClient({
defaultOptions:{
  queries:{
    retry: 1,
    refetchOnWindowFocus: false
  }
}
});
ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <App/>
      </AuthProvider>
    </QueryClientProvider>
  </React.StrictMode>
);
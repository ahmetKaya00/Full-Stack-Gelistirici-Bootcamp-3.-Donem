import type React from "react";
import { useAuth } from "./AuthContext";
import { Navigate } from "react-router-dom";

export default function ProtectedRoute({children}:{children: React.ReactNode}){
    const {isAuthentication} = useAuth();

    if(!isAuthentication)
        return <Navigate to="/login" replace/>;

    return<>{children}</>
}
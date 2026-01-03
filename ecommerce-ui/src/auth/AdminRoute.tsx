import type React from "react";
import { useAuth } from "./AuthContext";
import { Navigate } from "react-router-dom";


export default function AdminRoute({children}:{children: React.ReactNode}){
    const {isAuthentication,hasRole} = useAuth();
    if(!isAuthentication)
        return<Navigate to="/login" replace/>
    if(!hasRole("Admin"))
        return<Navigate to="/products" replace/>
    
    return<>{children}</>
}

import type { JSX } from "react";
import { useAuth } from "./AuthContext";
import { Navigate } from "react-router-dom";

export default function SellerRoute({children}:{children: JSX.Element}){

    const {user, isAuthentication} = useAuth();

    if(!isAuthentication){
        return <Navigate to="/login" replace/>
    }

    const isSeller = user?.roles.includes("Seller");
    if(!isSeller){
        return <Navigate to="/profile" replace/>
    }

    return children;
}
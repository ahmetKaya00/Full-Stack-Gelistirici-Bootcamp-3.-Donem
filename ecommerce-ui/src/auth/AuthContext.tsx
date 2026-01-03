import React, { createContext, useContext, useMemo, useState } from "react";
import { api } from "../api/client";
import { jwtDecode } from "jwt-decode";
type JwtPayloadAny = Record<string,any>;

export type AuthUser = {
    email?: string;
    fullName?: string;
    roles: string[];
};

type LoginDto = {
    email: string;
    password: string;
};

type AuthContextType = {
    token: string | null;
    isAuthentication: boolean;
    user: AuthUser | null;
    roles: string[];
    hasRole: (role:string) => boolean;
    login: (dto: LoginDto) => Promise<void>;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType>(null as any);

function normalizedRoles(payload: JwtPayloadAny): string[]{
    const claim = 
        payload.role ??
        payload.roles ??
        payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    
        if(!claim) return [];

        if(typeof claim === "string")return[claim];
        return[];
}
function readEmail(payload: JwtPayloadAny):string | undefined{
    return(
        payload.email ??
        payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"]??
        payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/emailaddress"]
    )
}
function readFullName(payload: JwtPayloadAny):string | undefined{
    return(
        payload.name ??
        payload.unique_name ??
        payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]??
        payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/name"]
    );
}
export function AuthProvider({children}: {children:React.ReactNode}){
    const [token, setToken] = useState<string | null>(localStorage.getItem("token"));

    const user = useMemo<AuthUser | null>(()=>{
        if(!token) return null;

        try{
            const payload = jwtDecode<JwtPayloadAny>(token);

            return{
                email: readEmail(payload),
                fullName: readFullName(payload),
                roles: normalizedRoles(payload),
            };
        }catch{
            return null;
        }
    }, [token]);
    const roles = user?.roles ?? [];
    const isAuthentication = !!token;
    const hasRole = (role:string) => roles.includes(role);

    const login = async(dto: LoginDto)=>{
        const res = await api.post("/api/Auth/login",dto);
        const newToken = res.data?.token ?? res.data?.accessToken ?? res.data?.jwt ?? null;

        if(!newToken || typeof newToken !== "string"){
            throw new Error("Login response içinde token bulunmamaktadır.");
        }
        localStorage.setItem("token",newToken);
        setToken(newToken);
    };
    const logout = () => {
        localStorage.removeItem("token");
        setToken(null);
    };
    return (
        <AuthContext.Provider value={{token,isAuthentication,user,roles,hasRole,login,logout}}>
            {children}
        </AuthContext.Provider>
    );
}
export function useAuth(){
    return useContext(AuthContext);
}

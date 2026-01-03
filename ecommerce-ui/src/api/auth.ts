import { api } from "./client";

export type RegisterDto = {
    fullName: string;
    email: string;
    password: string;
}
export type LoginDto = {
    email: string;
    password: string;
}

export async function register(dto:RegisterDto):Promise<any> {
    const {data} = await api.post("/api/Auth/register",dto);
    return data;
}

export async function login(dto:LoginDto):Promise<string> {
    const {data} = await api.post("/api/Auth/login",dto);
    if(typeof data === "string") return data;
    if(data?.token) return data.token;
    if(data?.accessToken) return data.accessToken;
    throw new Error("Login response token içerimiyor.");
}
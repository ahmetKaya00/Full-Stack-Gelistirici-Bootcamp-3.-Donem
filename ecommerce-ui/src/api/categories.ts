import { api } from "./client";

export type CategoryDto = {
    id: number;
    name:string;
};

export async function GetCategories():Promise<CategoryDto[]> {
 const res = await api.get("/api/categories");
 return res.data;   
}
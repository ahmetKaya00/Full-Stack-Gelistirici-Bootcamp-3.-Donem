import type { ProductDto } from "../types/products";
import { api } from "./client";

export async function getProducts():Promise<ProductDto[]> {
    const {data} = await api.get<ProductDto[]>("/api/Products");
    return data;
}
export async function getProduct(id:number):Promise<ProductDto[]> {
        const {data} = await api.get<ProductDto[]>(`/api/Products/${id}`);
        return data;
}
export async function createProduct(formData:FormData) {
    const res = await api.post("/api/Products",formData,{
        headers:{
            "Content-Type": "multipart/form-data",
        },
    });
    return res.data;
}
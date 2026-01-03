import { api } from "./client";

export type CartItemDto = {
    productId: number;
    productName?: string | null;
    unitPrice: number;
    quantity: number;
    lineTotal: number;
    imageUrl?: string | null;
};

export type CartSummaryDto = {
    items?: CartItemDto[] | null;
    totalPrice: number;
};

export async function getMyCart(): Promise<CartSummaryDto> {
    const {data} = await api.get("/api/Cart/my");
    return data;
}

export async function addToCart(productId:number,quantity:number) {
    const pid = Number(productId);
    const qty = Number(quantity);
    if(!Number.isFinite(pid)||pid<=0)
        throw new Error("Geçersiz ürün id");
    if(!Number.isFinite(qty)||qty<1)
        throw new Error("Adet 1 veya daha büyük olmalı");
    
    const fd = new FormData();

    fd.append("ProductId",String(pid));
    fd.append("Quantity",String(qty));
    const {data} = await api.post("/api/Cart/add",fd);
    return data;
}

export async function removeFromCart(productId:number, quantity?: number | null) {
    const pid = Number(productId);
    const fd = new FormData();
    fd.append("ProductId",String(pid));

    if(quantity != null)
        fd.append("Quantity",String(quantity));

    const {data} = await api.post("/api/Cart/remove",fd);
    return data;
}

export async function checkout() {
    const{data} = await api.post("/api/Cart/checout");
    return data;
}
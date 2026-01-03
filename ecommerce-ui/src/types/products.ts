export type ProductDto = {
    id: number;
    name?: string | null;
    stock: number;
    price: number;
    imageUrl?: string | null;
    description?: string | null;
    categoryName?: string | null;
    sellerShopName?: string | null;
}
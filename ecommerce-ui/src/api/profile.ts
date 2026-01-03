import { api } from "./client";

export type MeDto = {
    email?: string | null;
    fullName?: string | null;
    roles?: string | null;
    isSeller?: boolean;
    shopName?: string | null;
    description: string | null;
};

export type BecomeSellerDto = {
    shopName: string;
    description?: string;
}

export async function getMe():Promise<MeDto> {
    const{data} = await api.get("/api/profile/me");
    return data;
}
export async function becomeSeller(dto:BecomeSellerDto) {
    const{data} = await api.post("/api/profile/become-seller",dto);
    return data;
}
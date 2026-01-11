import React, { useState } from 'react'
import { useAuth } from '../../auth/AuthContext';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { becomeSeller, getMe } from '../../api/profile';


type SellerProfileDto = {
  id: number;
  shopName: string;
  description?: string | null;
  status: "Pending" | "Approved" | "Rejected" | string;
}
type MeDto = {
  fullName?:string | null;
  email?:string | null;
  sellerProfile?: SellerProfileDto | null;
}
export default function ProfilePage() {
  const {user} = useAuth();

  const qc = useQueryClient();

  const {data, isLoading,isError} = useQuery<MeDto>({
    queryKey: ["me"],
    queryFn: getMe as any
  });

  const [shopName,setShopName] = useState("");
  const [description, setDescription] = useState("");
  const becomeSellerMutation = useMutation({
    mutationFn: becomeSeller,
    onSuccess: async (msg) => {
      alert(typeof msg === "string" ? msg : "Başvuru Alındı");
      setShopName("");
      setDescription("");
      await qc.invalidateQueries({queryKey: ["me"]});
    },
    onError(err: any) {
      alert(err?.response?.data ?? err?.message??"Başvuru Gönderilmedi");
    },
  });
  if(isLoading)return<div className='text-slate-600'>Yükleniyor...</div>

  if(isError ||!data)
    return <div className='text-red-600'>Profil bilgileri alınamadı.</div>

  const sellerStatus = data.sellerProfile?.status??null;
  const isSellerApproved = sellerStatus === "Approved";
  const isSellerPending = sellerStatus === "Pending";
  const isSellerRejected = sellerStatus === "Rejected";
  return (
    <div className='max-w-3x1 space-y-6'>
      <div>
        <h1 className='text-2x1 font-semibold text-slate-900'>Profil</h1>
        <h1 className='text-sm text-slate-600'>Hesap ve satıcı bilgileri</h1>
      </div>
      <div className='rounded-2x1 border border-slate-200 bg-white p-6 shadow-sm'>
        <div className='space-y-2'>
          <div>
            <div className='text-xs text-slate-500'>Ad Soyad</div>
            <div className='font-medium text-slate-900'>{data.fullName ?? "-"}</div>
          </div>
          <div>
            <div className='text-xs text-slate-500'>E-posta</div>
            <div className='font-medium text-slate-900'>{data.email ?? "-"}</div>
          </div>
          <div>
            <div className='text-xs text-slate-500'>Roller</div>
            <div className='font-medium text-slate-900'>{(user?.roles ?? []).join(", ") || "-"}</div>
          </div>
        </div>
      </div>

      <div className='rounded-2xl border border-slate-200 bg-white p-6 shadow-sm'>
        <h2 className='text-lg font-semibold text-slate-900'>Satıcı Durumu</h2>

        {isSellerApproved&&(
          <div className='mt-4 rounded-xl bg-green-50 pd-4 text-green-700'>

            <div className='font-semibold'>Satıcı Onaylandı</div>
            <div className='mt-1 text-sm'>
              Mağaza: <strong>{data.sellerProfile?.shopName}</strong>
            </div>
            {data.sellerProfile?.description &&(
              <div className='mt-1 text-sm text-green-800'>
                {data.sellerProfile.description}
              </div>
            )}
          </div>
        )}
        {isSellerPending&&(
          <div className='mt-4 rounded-xl bg-yellow-50 pd-4 text-yellow-700'>

            <div className='font-semibold'>Başvuru Beklemede</div>
            
          </div>
        )}
        {isSellerRejected&&(
          <div className='mt-4 rounded-xl bg-red-50 pd-4 text-red-700'>

            <div className='font-semibold'>Satıcı Reddedildi</div>
            <div className='mt-1 text-sm'>
Tekrar Başvuru Yapabilirsin.
            </div>
          </div>
        )}
      </div>

      {!data.sellerProfile &&(
        <div className='mt-4 rounded-xl border border-slate-200 bg-white p-6'>
          <h3 className='text-lg font-semibold text-slate-900'>Satıcı Başvurusu</h3>

          <p className='mt-1 text-sm text-slate-600'>Satıcı olmak için mağaza bilgilerini gir.</p>

          <form className='mt-4 space-y-4' 
          onSubmit={(e)=>{

            e.preventDefault();
            becomeSellerMutation.mutate({
              shopName: shopName.trim(),
              description: description.trim()||undefined,
            });
          }}>
            <div>
              <label className='text-sm font-medium'>Mağaza Adı</label>
              <input className='mt-1 w-full rounded-xl border px-3 py-2' value={shopName} onChange={(e) => setShopName(e.target.value)} required placeholder='Ahmetin Bakkalı'/>
            </div>
            <div>
              <label className='text-sm font-medium'>Açıklama</label>
              <input className='mt-1 w-full rounded-xl border px-3 py-2' value={description} onChange={(e) => setDescription(e.target.value)} required placeholder='Ahmetin Bakkalı ürünleri'/>
            </div>

            <button disabled={becomeSellerMutation.isPending} className='w-full rounded-xl bg-slate-900 py-2 text-white font-semibold hover:bg-slate-800 disabled:opacity-60'>
              {becomeSellerMutation.isPending ? "Gönderiliyor..":"Satıcı Ol"}
            </button>
          </form>
        </div>
      )}
    </div>
  )
}

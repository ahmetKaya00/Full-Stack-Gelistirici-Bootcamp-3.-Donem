import React, { useState } from 'react'
import { useAuth } from '../../auth/AuthContext'
import { useNavigate, Link } from 'react-router-dom';

export default function LoginPage() {
  const {login} = useAuth();
  const nav = useNavigate();
  const [email,setEmail] = useState("");
  const [password,setPassword] = useState("");
  const [err,setErr] = useState<string | null>(null);
  const [loading,setLoading] = useState(false);

  const onSubmit = async (e:React.FormEvent) =>{
    e.preventDefault();
    setErr(null);
    setLoading(true);

    try {
      await login({email,password});
      nav("/products");
    } catch (ex:any) {
      setErr(ex?.response?.data?.message ?? ex?.message ?? "Giriş Başarısız.");
    }finally{
      setLoading(false);
    }
  }
  return (
    <div className="mx-auto mt-10 max-w-md">
      <div className='rounded-2x1 border border-slate-200 bg-white p-6 shadow-sm'>
        <h1 className='text-xl fomt-semibold text-slate-900'>Giriş</h1>
        <p className='mt-1 text-sm text-slate-600'>Devam etmek için hesabına giriş yap.</p>
        <form onSubmit={onSubmit} className='mt-5 grid gap-3'>
        <div>
          <label className='text-sm font-medium text-slate-700'>Email</label>
          <input className='mt-1 w-full rounded-xl border border-slate-200 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-200' value={email} onChange={(e) => setEmail(e.target.value)} placeholder='mail@ornek.com'></input>
        </div>
        <div>
          <label className='text-sm font-medium text-slate-700'>Şifre</label>
          <input className='mt-1 w-full rounded-xl border border-slate-200 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-200' value={password} onChange={(e) => setPassword(e.target.value)} placeholder='*********'></input>
        </div>
        {err &&(
          <div className='rounded-xl border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700'>{err}</div>
        )}
        <button disabled={loading} className='mt-2 rounded-xl bg-slate-900 px-4 py-2.5 text-sm font-semibold text-white hover:bg-slate-800 disabled:opacity-60'>
          {loading ? "Giriş yapılıyor...":"Giriş Yap"}
        </button>
        <div className='text-sm text-slate-600'>
          Hesabın Yok mu ?
          <Link className='font-medium text-slate-900 hover: underline' to="/register">Kayıt Ol</Link>
        </div>
        </form>
      </div>
    </div>
  )
}

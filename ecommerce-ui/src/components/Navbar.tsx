import { useQuery } from "@tanstack/react-query";
import { Link, NavLink } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { getMyCart } from "../api/cart";

function NavItem({ to, label }: { to: string; label: string }) {
  return (
    <NavLink
      to={to}
      className={({ isActive }) =>
        `text-sm font-medium transition ${
          isActive ? "text-slate-900" : "text-slate-600 hover:text-slate-900"
        }`
      }
    >
      {label}
    </NavLink>
  );
}

export default function Navbar() {
  const { isAuthentication, user, logout, hasRole } = useAuth();

  const { data: cart } = useQuery({
    queryKey: ["cart"],
    queryFn: getMyCart,
    enabled: isAuthentication,
  });

  const cartCount =
    cart?.items?.reduce((sum: number, i: any) => sum + (i.quantity ?? 0), 0) ?? 0;

  const initial =
    user?.fullName?.[0]?.toUpperCase() ??
    user?.email?.[0]?.toUpperCase() ??
    "U";

  return (
    <header className="sticky top-0 z-50 border-b border-slate-200 bg-white/80 backdrop-blur">
      <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-3">
        <Link to="/products" className="flex items-center gap-2">
          <div className="grid h-9 w-9 place-items-center rounded-xl bg-slate-900 font-bold text-white">
            E
          </div>
          <div className="leading-tight">
            <div className="font-semibold text-slate-900">ECommerce</div>
            <div className="text-xs text-slate-500">storeFront</div>
          </div>
        </Link>

        <nav className="flex items-center gap-5">
          <NavItem to="/products" label="Ürünler" />

          <NavLink
            to="/cart"
            className={({ isActive }) =>
              `relative text-sm font-medium transition ${
                isActive ? "text-slate-900" : "text-slate-600 hover:text-slate-900"
              }`
            }
          >
            Sepet
            {isAuthentication && cartCount > 0 && (
              <span className="ml-2 inline-flex min-w-[20px] items-center justify-center rounded-full bg-slate-900 px-2 py-0.5 text-xs font-semibold text-white">
                {cartCount}
              </span>
            )}
          </NavLink>

          {!isAuthentication ? (
            <>
              <NavItem to="/login" label="Giriş" />
              <NavItem to="/register" label="Kayıt" />
            </>
          ) : (
            <>
              {hasRole("Seller") && (
                <NavItem to="/seller/products/create" label="Ürün Ekle" />
              )}

              {hasRole("Admin") && <NavItem to="/admin/sellers" label="Admin" />}

              <Link
                to="/profile"
                className="flex items-center gap-2 rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
              >
                <div className="grid h-7 w-7 place-items-center rounded-full bg-slate-900 text-xs font-bold text-white">
                  {initial}
                </div>
                Profil
              </Link>

              <button
                onClick={logout}
                className="rounded-xl border border-slate-200 bg-white px-3 py-2 font-medium text-slate-700 hover:bg-slate-50"
              >
                Çıkış
              </button>
            </>
          )}
        </nav>
      </div>
    </header>
  );
}
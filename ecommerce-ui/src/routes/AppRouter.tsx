import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import Layout from "../components/Layout";
import ProductListPage from "../pages/products/ProductListPage";
import ProductDetailPage from "../pages/products/ProductDetailPage";
import LoginPage from "../pages/auth/LoginPage";
import RegisterPage from "../pages/auth/RegisterPage";
import ProtectedRoute from "../auth/ProtectedRoute";
import ProfilePage from "../pages/profile/ProfilePage";
import SellerRoute from "../auth/SellerRoute";
import SellerProductCreatePage from "../pages/seller/SellerProductCreatePage";
import AdminSellersPAge from "../pages/admin/AdminSellersPAge";
import AdminRoute from "../auth/AdminRoute";
import AdminDashboardPage from "../pages/admin/AdminDashboardPage";

export default function AppRouter() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<Navigate to="/products" replace />} />

          <Route path="products" element={<ProductListPage />} />
          <Route path="products/:id" element={<ProductDetailPage />} />

          <Route path="login" element={<LoginPage />} />
          <Route path="register" element={<RegisterPage />} />

          <Route
            path="profile"
            element={
              <ProtectedRoute>
                <ProfilePage />
              </ProtectedRoute>
            }
          />

          <Route
            path="seller/products/create"
            element={
              <SellerRoute>
                <SellerProductCreatePage />
              </SellerRoute>
            }
          />
          <Route
            path="admin/sellers"
            element={
              <AdminRoute>
                <AdminSellersPAge />
              </AdminRoute>
            }
          />

          <Route
            path="admin"
            element={
              <AdminRoute>
                <AdminDashboardPage />
              </AdminRoute>
            }
          />

          <Route path="*" element={<div>Sayfa Bulunamadı</div>} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
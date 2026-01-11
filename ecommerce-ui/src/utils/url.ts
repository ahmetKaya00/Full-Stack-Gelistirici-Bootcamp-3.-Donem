export function productImageUrl(imageUrl?: string | null) {
  if (!imageUrl) return null;

  const raw = imageUrl.trim();

  if (!raw) return null;
  if (raw.startsWith("http://") || raw.startsWith("https://")) return raw;

  const base = (import.meta.env.VITE_API_URL as string).replace(/\/+$/, "");

  if (raw.startsWith("/")) return `${base}${raw}`;

  if (raw.startsWith("uploads/") || raw.startsWith("uploads\\")) 
    return `${base}/${raw.replaceAll("\\", "/")}`;

  return `${base}/uploads/products/${raw}`;
}
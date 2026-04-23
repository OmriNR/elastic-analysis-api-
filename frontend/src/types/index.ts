export interface User {
  user_id: string;
  email: string;
  password: string;
  is_active: boolean;
  is_admin: boolean;
  created_at: string;
}

export interface GeoProperties {
  city: string;
  country: string;
  address: string;
  zip_code: string;
}

export interface UserProperties {
  user_id: string;
  user_name: string;
  age: number;
  gender: string;
  location: GeoProperties;
  created_at: string;
}

export interface Product {
  product_id: string;
  owner_id: string;
  name: string;
  description: string;
  category: string;
  sub_category?: string;
  price: number;
  quantity: number;
}

export interface Discount {
  discount_id: string;
  prodcut_id: string;
  percentage: number;
  expired_at: string;
}

export interface Order {
  order_id: string;
  timestamp: string;
  customer: UserProperties;
  total_amount: number;
  payment_method: string;
  discount_applied: boolean;
  items: Product[];
}

export interface CartItem {
  product: Product;
  quantity: number;
  discount?: Discount;
}

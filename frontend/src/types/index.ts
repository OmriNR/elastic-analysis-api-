export interface GeoProperties {
  city: string;
  country: string;
  address: string;
  zip_code: string;
}

export interface UserProperties {
  user_name: string;
  age: number;
  gender: string;
  location: GeoProperties;
}

export interface User {
  user_id: string;
  email: string;
  password: string;
  is_active: boolean;
  is_admin: boolean;
  created_at: string;
  properties: UserProperties;
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
  product_id: string;
  percentage: number;
  expired_at: string;
}

export interface Order {
  order_id: string;
  timestamp: string;
  customer: string;
  total_amount: number;
  payment_method: string;
  discount_applied: boolean;
  items: Product[];
}

export interface PublicUser {
  userId: string;
  userName: string;
  createdAt: string;
}

export interface CartItem {
  product: Product;
  quantity: number;
  discount?: Discount;
}

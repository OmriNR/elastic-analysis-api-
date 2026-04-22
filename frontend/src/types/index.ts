export interface User {
  userId: string;
  email: string;
  password: string;
  isActive: boolean;
  isAdmin: boolean;
  createdAt: string;
}

export interface GeoProperties {
  city: string;
  country: string;
  address: string;
  zipCode: string;
}

export interface UserProperties {
  userId: string;
  userName: string;
  age: number;
  gender: string;
  location: GeoProperties;
  createdAt: string;
}

export interface Product {
  productId: string;
  ownerId: string;
  name: string;
  description: string;
  category: string;
  subCategory?: string;
  price: number;
  quantity: number;
}

export interface Discount {
  discountId: string;
  prodcutId: string;
  percentage: number;
  expiredAt: string;
}

export interface Order {
  orderId: string;
  timestamp: string;
  customer: UserProperties;
  totalAmount: number;
  paymentMethod: string;
  discountApplied: boolean;
  items: Product[];
}

export interface CartItem {
  product: Product;
  quantity: number;
  discount?: Discount;
}

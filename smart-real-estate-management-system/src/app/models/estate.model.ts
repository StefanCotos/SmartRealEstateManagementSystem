export interface Estate {
  id?: string;
  userId: string;
  name: string;
  description: string;
  price: number;
  bedrooms: number;
  bathrooms: number;
  landSize: number;
  street: string;
  city: string;
  state: string;
  zipCode: string;
  houseSize: number;
  listingData: Date;
  productId?: string;
  priceId?: string;
  isSold?: boolean;
  buyerId?: string;
}

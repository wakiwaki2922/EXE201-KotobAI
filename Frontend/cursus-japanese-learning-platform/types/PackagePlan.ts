export type PackagePlan = {
  id: number;
  planType: string;
  planName: string;
  price: number; // Price for monthly
  period: number; // 'monthly' or 'yearly'
  isActive: boolean;
};

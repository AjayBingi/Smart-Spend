export interface User {
  token: string;
  username: string;
  email: string;
}

export interface Category {
  id: number;
  name: string;
  icon: string;
  color: string;
}

export interface Expense {
  id: number;
  title: string;
  amount: number;
  date: string;
  note?: string;
  source: string;
  userId: number;
  categoryId: number;
  categoryName: string;
  categoryIcon: string;
  categoryColor: string;
}

export interface Budget {
  id: number;
  limitAmount: number;
  month: number;
  year: number;
  userId: number;
  categoryId: number;
  categoryName: string;
  categoryIcon: string;
}

export interface AIInsight {
  id: number;
  insightText: string;
  type: string;
  generatedAt: string;
  userId: number;
}

export interface UserProfile {
  id: number;
  userId: number;
  monthlyIncome: number;
  rentAmount: number;
  homeSavingsAmount: number;
}

export interface CategorySpending {
  categoryName: string;
  color: string;
  icon: string;
  amount: number;
  percentage: number;
}

export interface MonthlySpending {
  month: string;
  year: number;
  amount: number;
}

export interface BudgetStatus {
  categoryName: string;
  icon: string;
  budgeted: number;
  spent: number;
  remaining: number;
  isOverBudget: boolean;
}

export interface Anomaly {
  categoryName: string;
  icon: string;
  budgeted: number;
  spent: number;
  overBy: number;
}

export interface AuthResponse {
  token: string;
  username: string;
  email: string;
}
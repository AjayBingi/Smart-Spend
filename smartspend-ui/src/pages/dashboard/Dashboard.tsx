import { useEffect, useState } from 'react';
import api from '../../api/axiosInstance';
import { Expense, BudgetStatus, CategorySpending } from '../../types/index';
import styles from '../../styles/Dashboard.module.css';

const Dashboard = () => {
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [budgetStatus, setBudgetStatus] = useState<BudgetStatus[]>([]);
  const [categorySpending, setCategorySpending] = useState<CategorySpending[]>([]);
  const [loading, setLoading] = useState(true);

  const currentMonth = new Date().getMonth() + 1;
  const currentYear = new Date().getFullYear();

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [expensesRes, budgetRes, categoryRes] = await Promise.all([
          api.get<Expense[]>('/api/expenses'),
          api.get<BudgetStatus[]>(`/api/analytics/budget-status?month=${currentMonth}&year=${currentYear}`),
          api.get<CategorySpending[]>(`/api/analytics/by-category?month=${currentMonth}&year=${currentYear}`)
        ]);
        setExpenses(expensesRes.data);
        setBudgetStatus(budgetRes.data);
        setCategorySpending(categoryRes.data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const totalSpent = expenses
    .filter(e => new Date(e.date).getMonth() + 1 === currentMonth)
    .reduce((sum, e) => sum + e.amount, 0);

  const overBudgetCount = budgetStatus.filter(b => b.isOverBudget).length;
  const recentExpenses = [...expenses].slice(0, 5);

  if (loading) return <div className={styles.loading}>Loading...</div>;

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Dashboard</h1>

      {/* Summary Cards */}
      <div className={styles.cards}>
        <div className={styles.card}>
          <p className={styles.cardLabel}>Total Spent This Month</p>
          <p className={styles.cardValue}>₹{totalSpent.toFixed(2)}</p>
        </div>
        <div className={styles.card}>
          <p className={styles.cardLabel}>Top Category</p>
          <p className={styles.cardValue}>
            {categorySpending[0] ? `${categorySpending[0].icon} ${categorySpending[0].categoryName}` : '—'}
          </p>
        </div>
        <div className={styles.card}>
          <p className={styles.cardLabel}>Over Budget</p>
          <p className={`${styles.cardValue} ${overBudgetCount > 0 ? styles.danger : ''}`}>
            {overBudgetCount} {overBudgetCount === 1 ? 'category' : 'categories'}
          </p>
        </div>
        <div className={styles.card}>
          <p className={styles.cardLabel}>Total Expenses</p>
          <p className={styles.cardValue}>{expenses.length}</p>
        </div>
      </div>

      {/* Recent Expenses */}
      <div className={styles.section}>
        <h2 className={styles.sectionTitle}>Recent Expenses</h2>
        {recentExpenses.length === 0 ? (
          <p className={styles.empty}>No expenses yet. Start adding some!</p>
        ) : (
          <div className={styles.expenseList}>
            {recentExpenses.map(expense => (
              <div key={expense.id} className={styles.expenseItem}>
                <span className={styles.expenseIcon}>{expense.categoryIcon}</span>
                <div className={styles.expenseInfo}>
                  <p className={styles.expenseTitle}>{expense.title}</p>
                  <p className={styles.expenseCategory}>{expense.categoryName}</p>
                </div>
                <p className={styles.expenseAmount}>₹{expense.amount.toFixed(2)}</p>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Budget Status */}
      <div className={styles.section}>
        <h2 className={styles.sectionTitle}>Budget Status</h2>
        {budgetStatus.length === 0 ? (
          <p className={styles.empty}>No budgets set for this month.</p>
        ) : (
          <div className={styles.budgetList}>
            {budgetStatus.map(b => (
              <div key={b.categoryName} className={styles.budgetItem}>
                <div className={styles.budgetHeader}>
                  <span>{b.icon} {b.categoryName}</span>
                  <span className={b.isOverBudget ? styles.danger : styles.safe}>
                    ₹{b.spent.toFixed(2)} / ₹{b.budgeted.toFixed(2)}
                  </span>
                </div>
                <div className={styles.progressBar}>
                  <div
                    className={`${styles.progressFill} ${b.isOverBudget ? styles.progressDanger : styles.progressSafe}`}
                    style={{ width: `${Math.min((b.spent / b.budgeted) * 100, 100)}%` }}
                  />
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default Dashboard;
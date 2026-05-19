import { useEffect, useState } from 'react';
import api from '../../api/axiosInstance';
import { Budget, Category } from '../../types/index';
import styles from '../../styles/Budgets.module.css';

const Budgets = () => {
  const [budgets, setBudgets] = useState<Budget[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingBudget, setEditingBudget] = useState<Budget | null>(null);

  const [limitAmount, setLimitAmount] = useState('');
  const [month, setMonth] = useState(new Date().getMonth() + 1);
  const [year, setYear] = useState(new Date().getFullYear());
  const [categoryId, setCategoryId] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const [budgetsRes, categoriesRes] = await Promise.all([
        api.get<Budget[]>('/api/budgets'),
        api.get<Category[]>('/api/categories')
      ]);
      setBudgets(budgetsRes.data);
      setCategories(categoriesRes.data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const resetForm = () => {
    setLimitAmount('');
    setMonth(new Date().getMonth() + 1);
    setYear(new Date().getFullYear());
    setCategoryId('');
    setError('');
    setEditingBudget(null);
    setShowForm(false);
  };

  const handleEdit = (budget: Budget) => {
    setEditingBudget(budget);
    setLimitAmount(budget.limitAmount.toString());
    setMonth(budget.month);
    setYear(budget.year);
    setCategoryId(budget.categoryId.toString());
    setShowForm(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    const payload = {
      limitAmount: parseFloat(limitAmount),
      month,
      year,
      categoryId: parseInt(categoryId)
    };

    try {
      if (editingBudget) {
        await api.put(`/api/budgets/${editingBudget.id}`, payload);
      } else {
        await api.post('/api/budgets', payload);
      }
      await fetchData();
      resetForm();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Something went wrong.');
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Delete this budget?')) return;
    try {
      await api.delete(`/api/budgets/${id}`);
      await fetchData();
    } catch (err) {
      console.error(err);
    }
  };

  if (loading) return <div className={styles.loading}>Loading...</div>;

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.title}>Budgets</h1>
        <button className={styles.addBtn} onClick={() => setShowForm(true)}>
          + Add Budget
        </button>
      </div>

      {showForm && (
        <div className={styles.formCard}>
          <h2>{editingBudget ? 'Edit Budget' : 'Add Budget'}</h2>
          {error && <p className={styles.error}>{error}</p>}
          <form onSubmit={handleSubmit} className={styles.form}>
            <div className={styles.formGrid}>
              <div className={styles.field}>
                <label>Category</label>
                <select value={categoryId} onChange={e => setCategoryId(e.target.value)} required>
                  <option value="">Select category</option>
                  {categories.map(c => (
                    <option key={c.id} value={c.id}>{c.icon} {c.name}</option>
                  ))}
                </select>
              </div>
              <div className={styles.field}>
                <label>Limit Amount (₹)</label>
                <input type="number" value={limitAmount} onChange={e => setLimitAmount(e.target.value)} placeholder="0.00" required />
              </div>
              <div className={styles.field}>
                <label>Month</label>
                <select value={month} onChange={e => setMonth(parseInt(e.target.value))}>
                  {Array.from({ length: 12 }, (_, i) => (
                    <option key={i + 1} value={i + 1}>
                      {new Date(2000, i).toLocaleString('default', { month: 'long' })}
                    </option>
                  ))}
                </select>
              </div>
              <div className={styles.field}>
                <label>Year</label>
                <input type="number" value={year} onChange={e => setYear(parseInt(e.target.value))} required />
              </div>
            </div>
            <div className={styles.formActions}>
              <button type="submit" className={styles.saveBtn}>
                {editingBudget ? 'Update' : 'Save'}
              </button>
              <button type="button" className={styles.cancelBtn} onClick={resetForm}>
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      {budgets.length === 0 ? (
        <p className={styles.empty}>No budgets yet. Start adding some!</p>
      ) : (
        <div className={styles.table}>
          <div className={styles.tableHeader}>
            <span>Category</span>
            <span>Month</span>
            <span>Year</span>
            <span>Limit</span>
            <span>Actions</span>
          </div>
          {budgets.map(budget => (
            <div key={budget.id} className={styles.tableRow}>
              <span>{budget.categoryIcon} {budget.categoryName}</span>
              <span>{new Date(2000, budget.month - 1).toLocaleString('default', { month: 'long' })}</span>
              <span>{budget.year}</span>
              <span>₹{budget.limitAmount.toFixed(2)}</span>
              <span className={styles.actions}>
                <button className={styles.editBtn} onClick={() => handleEdit(budget)}>Edit</button>
                <button className={styles.deleteBtn} onClick={() => handleDelete(budget.id)}>Delete</button>
              </span>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default Budgets;
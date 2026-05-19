import { useEffect, useState } from 'react';
import { PieChart, Pie, Cell, BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, Legend } from 'recharts';
import api from '../../api/axiosInstance';
import { CategorySpending, MonthlySpending, BudgetStatus, Anomaly } from '../../types/index';
import styles from '../../styles/Analytics.module.css';

const Analytics = () => {
    const [categorySpending, setCategorySpending] = useState<CategorySpending[]>([]);
    const [monthlySpending, setMonthlySpending] = useState<MonthlySpending[]>([]);
    const [budgetStatus, setBudgetStatus] = useState<BudgetStatus[]>([]);
    const [anomalies, setAnomalies] = useState<Anomaly[]>([]);
    const [loading, setLoading] = useState(true);

    const currentMonth = new Date().getMonth() + 1;
    const currentYear = new Date().getFullYear();

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        try {
            const [categoryRes, monthlyRes, budgetRes, anomalyRes] = await Promise.all([
                api.get<CategorySpending[]>(`/api/analytics/by-category?month=${currentMonth}&year=${currentYear}`),
                api.get<MonthlySpending[]>(`/api/analytics/monthly?month=${currentMonth}&year=${currentYear}`),
                api.get<BudgetStatus[]>(`/api/analytics/budget-status?month=${currentMonth}&year=${currentYear}`),
                api.get<Anomaly[]>(`/api/analytics/anomalies?month=${currentMonth}&year=${currentYear}`)
            ]);
            setCategorySpending(categoryRes.data);
            setMonthlySpending(monthlyRes.data);
            setBudgetStatus(budgetRes.data);
            setAnomalies(anomalyRes.data);
        } catch (err) {
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <div className={styles.loading}>Loading...</div>;

    return (
        <div className={styles.container}>
            <h1 className={styles.title}>Analytics</h1>

            <div className={styles.grid}>
                {/* Pie Chart */}
                <div className={styles.card}>
                    <h2 className={styles.cardTitle}>Spending by Category</h2>
                    {categorySpending.length === 0 ? (
                        <p className={styles.empty}>No data for this month.</p>
                    ) : (
                        <ResponsiveContainer width="100%" height={300}>
                            <PieChart>
                                <Pie data={categorySpending} dataKey="amount" nameKey="categoryName" cx="50%" cy="50%" outerRadius={100} label={({ name, value }) => `${name} ${((value as number) / categorySpending.reduce((sum, c) => sum + c.amount, 0) * 100).toFixed(1)}%`}>
                                    {categorySpending.map((entry, index) => (
                                        <Cell key={index} fill={entry.color} />
                                    ))}
                                </Pie>
                                <Tooltip formatter={(value) => {
                                    return `₹${Number(value).toFixed(2)}`;
                                }} />
                            </PieChart>
                        </ResponsiveContainer>
                    )}
                </div>

                {/* Bar Chart */}
                <div className={styles.card}>
                    <h2 className={styles.cardTitle}>Monthly Trend</h2>
                    {monthlySpending.length === 0 ? (
                        <p className={styles.empty}>No data available.</p>
                    ) : (
                        <ResponsiveContainer width="100%" height={300}>
                            <BarChart data={monthlySpending}>
                                <XAxis dataKey="month" />
                                <YAxis />
                                <Tooltip formatter={(value) => {
                                    return `₹${Number(value).toFixed(2)}`;
                                }} />
                                <Bar dataKey="amount" fill="#4f46e5" radius={[4, 4, 0, 0]} />
                            </BarChart>
                        </ResponsiveContainer>
                    )}
                </div>

                {/* Budget Status */}
                <div className={styles.card}>
                    <h2 className={styles.cardTitle}>Budget Status</h2>
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

                {/* Anomalies */}
                <div className={styles.card}>
                    <h2 className={styles.cardTitle}>⚠️ Anomalies</h2>
                    {anomalies.length === 0 ? (
                        <p className={styles.empty}>No anomalies detected. Great job staying on budget!</p>
                    ) : (
                        <div className={styles.anomalyList}>
                            {anomalies.map(a => (
                                <div key={a.categoryName} className={styles.anomalyItem}>
                                    <span>{a.icon} {a.categoryName}</span>
                                    <div className={styles.anomalyDetails}>
                                        <span>Budgeted: ₹{a.budgeted.toFixed(2)}</span>
                                        <span>Spent: ₹{a.spent.toFixed(2)}</span>
                                        <span className={styles.danger}>Over by: ₹{a.overBy.toFixed(2)}</span>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Analytics;
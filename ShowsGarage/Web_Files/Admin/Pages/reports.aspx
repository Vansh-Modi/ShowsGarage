<%@ Page Title="P/L Analytics Report | Admin" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="report.aspx.cs" Inherits="ShowsGarage.Web_Files.Admin.admin_pl_report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>P&L Analytics Dashboard | Admin Hub</title>
    <link href="/Web_Files/Admin/Styles/siteSettings.css?v=2" rel="stylesheet" type="text/css" />
    <link href="/Web_Files/Admin/Styles/report.css?v=2" rel="stylesheet" type="text/css" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="admin-theme-wrapper">
        <main class="admin-content-container wide-fluid-layout">
            
            <div class="admin-controls-bar">
                <h1 class="admin-main-heading">Business Intelligence & P/L Analytics</h1>
                <span class="admin-pill-badge badge-financial">Comprehensive Store Audit</span>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="admin-alert-banner alert-error" Visible="false"></asp:Label>

            <div class="pl-overview-cards-grid">
                <div class="pl-stat-card card-revenue">
                    <div class="stat-meta-header"><span class="stat-label">Gross Income Sales</span><span class="stat-icon">💰</span></div>
                    <div class="stat-amount-display">Rs.<asp:Label ID="lblTotalRevenue" runat="server" Text="0.00"></asp:Label></div>
                    <p class="stat-desc-hint">Approved billing collection streams.</p>
                </div>

                <div class="pl-stat-card card-expenses">
                    <div class="stat-meta-header"><span class="stat-label">Total Outflow Costs</span><span class="stat-icon">💸</span></div>
                    <div class="stat-amount-display">Rs.<asp:Label ID="lblTotalExpenses" runat="server" Text="0.00"></asp:Label></div>
                    <p class="stat-desc-hint">Sourcing costs, logistics & refunds.</p>
                </div>

                <div id="divNetProfitCard" runat="server" class="pl-stat-card card-net-margin">
                    <div class="stat-meta-header"><span class="stat-label">Net Performance Margin</span><asp:Label ID="lblNetStatusIcon" runat="server" CssClass="stat-icon" Text="📊"></asp:Label></div>
                    <div class="stat-amount-display">Rs.<asp:Label ID="lblNetMargin" runat="server" Text="0.00"></asp:Label></div>
                    <p class="stat-desc-hint">Net operational profitability indicator.</p>
                </div>
            </div>

            <div class="operational-summary-strip-row">
                <div class="strip-node">Total Orders Volume: <strong><asp:Literal ID="litTotalOrdersCount" runat="server" Text="0"></asp:Literal></strong></div>
                <div class="strip-node">Logistics Fees Collected: <strong class="text-green">Rs.<asp:Literal ID="litTotalShippingFees" runat="server" Text="0.00"></asp:Literal></strong></div>
                <div class="strip-node">Inventory Sourcing Cost: <strong class="text-red">Rs.<asp:Literal ID="litTotalInventorySourcingCost" runat="server" Text="0.00"></asp:Literal></strong></div>
            </div>

            <div class="analytics-charts-split-grid">
                
                <div class="settings-card-panel chart-wrapper-card">
                    <h3 class="chart-card-title">Financial Vector Comparisons</h3>
                    <div class="canvas-chart-container">
                        <canvas id="financialComparisonBarChart"></canvas>
                    </div>
                </div>

                <div class="settings-card-panel chart-wrapper-card">
                    <h3 class="chart-card-title">Fulfillment Pipelines Segmentation</h3>
                    <div class="canvas-chart-container">
                        <canvas id="orderPipelinePieChart"></canvas>
                    </div>
                </div>

            </div>

            <div class="pl-breakdown-container-panel" style="margin-top: 25px;">
                <div class="settings-card-panel premium-border">
                    <h2 class="settings-card-title">Categorized Expense Distribution Ledger</h2>
                    <div class="pl-table-responsive-wrapper">
                        <asp:Repeater ID="rptExpenseTypeBreakdown" runat="server">
                           <HeaderTemplate>
                                <table class="admin-pl-breakdown-table">
                                    <thead>
                                        <tr>
                                            <th>Expense Category Allocation Type</th>
                                            <th style="text-align: right; width: 250px;">Total Outflow Accumulation</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="pl-cell-category"><span class="pl-bullet-accent"></span><%# Eval("ExpenseType") %></td>
                                    <td class="pl-cell-value">Rs.<%# string.Format("{0:N2}", Eval("CategoryTotal")) %></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                    </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>

        </main>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", function () {
            // Render Bar Chart Node Graphics
            const ctxBar = document.getElementById('financialComparisonBarChart').getContext('2d');
            new Chart(ctxBar, {
                type: 'bar',
                data: {
                    labels: ['Gross Sales', 'Sourcing Cost Outlays', 'Operational Expenses', 'Logistics Collected'],
                    datasets: [{
                        label: 'Valuation Metrics (Rs.)',
                        data: [
                            <%= ValueGrossSales %>, 
                            <%= ValueSourcingCost %>, 
                            <%= ValueTotalExpenses %>, 
                            <%= ValueShippingCollected %>
                        ],
                        backgroundColor: [
                            'rgba(46, 189, 89, 0.45)',  /* Green */
                            'rgba(244, 67, 54, 0.45)',   /* Red */
                            'rgba(255, 152, 0, 0.45)',   /* Orange */
                            'rgba(0, 118, 223, 0.45)'    /* Blue */
                        ],
                        borderColor: ['#2ebd59', '#f44336', '#ff9800', '#0076df'],
                        borderWidth: 1.5,
                        borderRadius: 4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: {
                        y: { grid: { color: '#222' }, ticks: { color: '#aaa', font: { family: 'monospace' } } },
                        x: { grid: { display: false }, ticks: { color: '#aaa' } }
                    }
                }
            });

            // Render Pie Chart Node Pipelines Graphics
            const ctxPie = document.getElementById('orderPipelinePieChart').getContext('2d');
            new Chart(ctxPie, {
                type: 'pie',
                data: {
                    labels: ['Approved / Settled', 'Awaiting Verification', 'Awaiting Payment', 'Cancelled Orders'],
                    datasets: [{
                        data: [
                            <%= CountApproved %>, 
                            <%= CountAwaitingVerify %>, 
                            <%= CountAwaitingPay %>, 
                            <%= CountCancelled %>
                        ],
                        backgroundColor: [
                            '#2ebd59', /* Green */
                            '#0076df', /* Blue */
                            '#ffc107', /* Yellow */
                            '#333333'  /* Dark Gray */
                        ],
                        borderWidth: 0
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: { position: 'right', labels: { color: '#ccc', font: { size: 12 } } }
                    }
                }
            });
        });
    </script>
</asp:Content>
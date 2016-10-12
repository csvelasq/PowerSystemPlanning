import plotly
from plotly.graph_objs import Scatter, Layout
import csv
import pandas as pd

print 'starting'
#read pareto front data
data = pd.read_csv('ParetoFrontData.txt')
efficient_alternatives_data = data.loc[data['Type']=='Efficient']
dominated_alternatives_data = data.loc[data['Type']=='Dominated']
#print data
#print efficient_alternatives_data 
#print dominated_alternatives_data 
            
#plot efficient alternatives and frontier
x_efficient = efficient_alternatives_data['X'].tolist()
y_efficient = efficient_alternatives_data['Y'].tolist()
ids_efficient = efficient_alternatives_data['PlanId'].tolist()
trace_efficient = Scatter(
    x=x_efficient, 
    y=y_efficient, 
    name='Efficient',
    mode='lines+markers',
    marker=dict(
        size=10
        ),
    text = ids_efficient
    )
#plot efficient frontier

#plot dominated alternatives
x_dominated = dominated_alternatives_data['X'].tolist()
y_dominated = dominated_alternatives_data['Y'].tolist()
ids_dominated = dominated_alternatives_data['PlanId'].tolist()
trace_dominated = Scatter(
    x=x_dominated, 
    y=y_dominated, 
    mode='markers',
    name='Dominated',
    marker=dict(
        size=5,
        color='rgb(200,200,200)'
        ),
    text = ids_dominated
    )
#render plot
plot_layout = Layout(
    title="Pareto Front for TEP under Scenarios",
    hovermode='closest',
    xaxis=dict(
        title='Total Cost Scenario 1'
        ),
    yaxis=dict(
        title='Total Cost Scenario 2'
        )
    )
plotly.offline.plot({
    "data": [trace_efficient,trace_dominated],
    "layout": plot_layout
})
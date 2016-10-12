import plotly
from plotly.graph_objs import Scatter, Layout
import csv
import pandas as pd

#read pareto front data
data = pd.read_csv('ParetoFrontData.txt') #csv format: PlanId,Type,X,Y; Type in ('Efficient','Dominated')
efficient_alternatives_data = data.loc[data['Type']=='Efficient']
dominated_alternatives_data = data.loc[data['Type']=='Dominated']

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
show_grid_lines = True
plot_layout = Layout(
    title="Pareto Front for TEP under Scenarios",
    hovermode='closest',
    xaxis=dict(
        title='Total Cost Scenario 1',
        showgrid=show_grid_lines
        ),
    yaxis=dict(
        title='Total Cost Scenario 2',
        showgrid=show_grid_lines
        )
    )
plotly.offline.plot({
    "data": [trace_efficient,trace_dominated],
    "layout": plot_layout
})
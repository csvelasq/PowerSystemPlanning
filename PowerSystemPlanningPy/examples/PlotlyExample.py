import plotly
from plotly.graph_objs import Scatter, Layout

#plotly.offline.plot({
#    "data": [Scatter(x=[1, 2, 4.5, 5, 8, 10.5, 1.5, 2.3, 3.2], y=[10, 6.5, 5, 3, 2.5, 2.5, 10.5, 7, 9], mode='markers')],
#    "layout": Layout(title="hello world")
#})

trace0 = Scatter(x=[1, 2, 4.5, 5, 8, 10.5, 1.5, 2.3, 3.2], y=[10, 6.5, 5, 3, 2.5, 2.5, 10.5, 7, 9], mode='markers')
plotly.offline.plot({
    "data": [trace0],
    "layout": Layout(title="Pareto Front")
})

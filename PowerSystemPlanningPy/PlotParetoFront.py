import plotly
from plotly.graph_objs import Scatter, Layout
import csv

#read pareto front data
xAll=[]
yAll=[]
with open('ParetoFrontData.txt', 'rb') as csvfile:
    spamreader = csv.reader(csvfile, delimiter=',') #, quotechar='|')
    for row in spamreader:
        xAll.append(float(row[0]))
        yAll.append(float(row[1]))

#plot
trace0 = Scatter(x=xAll, y=yAll, mode='markers')
plotly.offline.plot({
    "data": [trace0],
    "layout": Layout(title="Pareto Front")
})
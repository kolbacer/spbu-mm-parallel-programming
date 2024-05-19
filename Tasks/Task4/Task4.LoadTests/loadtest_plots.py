import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import os
import sys


csv_file = sys.argv[1]
set_type = sys.argv[2]
save_folder = sys.argv[3]
if not os.path.exists(save_folder):
    os.makedirs(save_folder)

df = pd.read_csv(csv_file)

metric = "http_req_duration"

load_30rps_add = (df.loc[(df['scenario'] == "load_30rps_add") & (df['metric_name'] == metric)])["metric_value"]
load_30rps_add.name = "30"
load_30rps_contains = (df.loc[(df['scenario'] == "load_30rps_contains") & (df['metric_name'] == metric)])["metric_value"]
load_30rps_contains.name = "30"
load_30rps_remove = (df.loc[(df['scenario'] == "load_30rps_remove") & (df['metric_name'] == metric)])["metric_value"]
load_30rps_remove.name = "30"
load_100rps_add = (df.loc[(df['scenario'] == "load_100rps_add") & (df['metric_name'] == metric)])["metric_value"]
load_100rps_add.name = "100"
load_100rps_contains = (df.loc[(df['scenario'] == "load_100rps_contains") & (df['metric_name'] == metric)])["metric_value"]
load_100rps_contains.name = "100"
load_100rps_remove = (df.loc[(df['scenario'] == "load_100rps_remove") & (df['metric_name'] == metric)])["metric_value"]
load_100rps_remove.name = "100"

df_load_add = pd.concat([load_30rps_add, load_100rps_add], axis=1)
df_load_contains = pd.concat([load_30rps_contains, load_100rps_contains], axis=1)
df_load_remove = pd.concat([load_30rps_remove, load_100rps_remove], axis=1)

boxplot_add = sns.boxplot(data=df_load_add)
boxplot_add.set_title(f'Add ({set_type})')
boxplot_add.set_xlabel('Requests per second')
boxplot_add.set_ylabel('Response time, ms')

plt.yscale('log')
fig_add = boxplot_add.get_figure()
fig_add.savefig(f"{save_folder}boxplot_add.png")
plt.clf()

boxplot_contains = sns.boxplot(data=df_load_contains)
boxplot_contains.set_title(f'Contains ({set_type})')
boxplot_contains.set_xlabel('Requests per second')
boxplot_contains.set_ylabel('Response time, ms')

plt.yscale('log')
fig_contains = boxplot_contains.get_figure()
fig_contains.savefig(f"{save_folder}boxplot_contains.png")
plt.clf()

boxplot_remove = sns.boxplot(data=df_load_remove)
boxplot_remove.set_title(f'Remove ({set_type})')
boxplot_remove.set_xlabel('Requests per second')
boxplot_remove.set_ylabel('Response time, ms')

plt.yscale('log')
fig_remove = boxplot_remove.get_figure()
fig_remove.savefig(f"{save_folder}boxplot_remove.png")
plt.clf()
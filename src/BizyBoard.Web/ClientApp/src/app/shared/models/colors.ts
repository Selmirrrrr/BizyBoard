export interface IChartColor {
    backgroundColor: string;
    borderColor: string;
    pointBackgroundColor: string;
    pointBorderColor: string;
    pointHoverBackgroundColor: string;
    pointHoverBorderColor: string;
    borderWidth: number;
}

export class ChartColors {
    public static Green: IChartColor = {
        backgroundColor: 'rgba(35,169,138,0.2)',
        borderColor: '#23a98a',
        pointBackgroundColor: '#23994a',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#23994a',
        pointHoverBorderColor: 'fff',
        borderWidth: 1
    };

    public static Blue: IChartColor = {
        backgroundColor: 'rgba(35,132,138,0.2)',
        borderColor: '#158BB6',
        pointBackgroundColor: '#1782B4',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#1782B4',
        pointHoverBorderColor: 'fff',
        borderWidth: 1
    };
}



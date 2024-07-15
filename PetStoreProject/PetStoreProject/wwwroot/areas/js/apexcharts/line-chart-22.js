(function ($) {

    var tfLineChart = (function () {

        var chartBar = function (days, dataProduct, dataService) {

            var options = {
                series: [{
                    name: 'Sản Phẩm',
                    data: dataProduct
                }, {
                    name: 'Dịch vụ',
                    data: dataService
                }],
                chart: {
                    height: 373,
                    type: 'area',
                    toolbar: {
                        show: false,
                    },
                },
                dataLabels: {
                    enabled: false
                },
                legend: {
                    show: false,
                },
                colors: ['#8D79F6', '#2377FC'],
                stroke: {
                    curve: 'smooth'
                },
                yaxis: {
                    show: false,
                },
                xaxis: {
                    labels: {
                        style: {
                            colors: '#95989D',
                        },
                    },
                    categories: days
                },
                tooltip: {
                    x: {
                        format: 'dd/mm/yy'
                    },
                },
            };

            chart = new ApexCharts(
                document.querySelector("#line-chart-22"),
                options
            );
            if ($("#line-chart-22").length > 0) {
                chart.render();
            }
        };

        /* Function ============ */
        return {
            init: function () { },

            load: function (days, dataProduct, dataService) {
                chartBar(days, dataProduct, dataService);
            },
            resize: function () { },
        };
    })();

    //jQuery(document).ready(function () { });

    jQuery(window).on("load", function () {
        var currentMonth = $("#month").val();
        var currentYear = $("#year").val();
        var data = getDaysInMonth(currentMonth, currentYear);
        tfLineChart.load(data.days, data.dataProduct, data.dataService);
    });

    jQuery(window).on("resize", function () { });
})(jQuery);
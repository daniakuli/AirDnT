$(function () {
    if (data) {
        var margin = {
            top: 20,
            right: 20,
            bottom: 50,
            left: 40
        },
            width = 500 - margin.left - margin.right,
            height = 500 - margin.top - margin.bottom;

        var x = d3.scale.ordinal()
            .rangeRoundBands([width, 0], 0.1);

        var y = d3.scale.linear()
            .range([0, height]);

        var xAxis = d3.svg.axis()
            .scale(x)
            .orient("bottom");

        var yAxis = d3.svg.axis()
            .scale(y)
            .orient("left")
            .tickFormat(d3.format("d"))
            .tickSubdivide(0);


        var svg = d3.select("svg#barChart").attr({
            width: width + margin.left + margin.right,
            height: height + margin.top + margin.bottom
        })
            .append("g")
            .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

        x.domain(data.map(function (d) {
            return d.key;
        }));

        y.domain([d3.max(data, function (d) {
            return d.value;
        }), 0]);

        svg.append("g").attr({
            class: "y axis",
            transform: "translate(0," + height + ")"
        })
            .call(xAxis)
            .selectAll("text")
            .attr({
                transform: "rotate(90)",
                x: 0,
                y: -6,
                dx: ".6em"
            })
        
            .style("text-anchor", "start");

        svg.append("g")
            .attr("class", "y axis")
            .call(yAxis);

        svg.selectAll(".bar")
            .data(data)
            .enter().append("rect")
            .attr({
                class: "bar",
                x: function (d) { return x(d.key); },
                width: x.rangeBand(),
                y: function (d) { return y(d.value); },
                height: function (d) { return height - y(d.value) }
            })

        for (var i = 0; i < data.length; i++) {
            var par = document.createElement('p');
            par.innerHTML = "Country: " + data[i].key + ", " + title + ": " + data[i].value;
            document.getElementById("details").appendChild(par);
        }
    }
})
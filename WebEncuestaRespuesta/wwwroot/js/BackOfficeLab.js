window.backOfficeLab = {

    getClientIp: async function () {
        try {
            const cached = sessionStorage.getItem("clientIp");

            if (cached) return cached;

            const response = await fetch("https://api.ipify.org?format=json");
            const data = await response.json();

            sessionStorage.setItem("clientIp", data.ip);
            return data.ip;
        }
        catch {
            return "N/A";
        }
    },

    getOS: function () {
        const ua = navigator.userAgent.toLowerCase();
        let os = "Desconocido";

        if (ua.includes("win")) os = "Windows";
        else if (ua.includes("mac")) os = "MacOS";
        else if (ua.includes("linux")) os = "Linux";
        else if (ua.includes("android")) os = "Android";
        else if (ua.includes("iphone") || ua.includes("ipad")) os = "iOS";

        sessionStorage.setItem("clientOS", os);
        return os;
    },

    getValue: function (key) {
        return sessionStorage.getItem(key);
    },

    setValue: function (key, value) {
        sessionStorage.setItem(key, value);
    }
};


    window.downloadFile = (fileName, base64, contentType) => {
        const link = document.createElement('a');
    link.href = `data:${contentType};base64,${base64}`;
        link.download = fileName;
        link.click();
    };


    window.renderDashboardChart = (labels, data) => {
        const ctx = document.getElementById("chartVisitas");

        new Chart(ctx, {
            type: "line",
            data: {
                labels: labels,
                datasets: [{
                    label: "Visitas",
                    data: data,
                    borderWidth: 2,
                    tension: 0.3
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: false }
                }
            }
        });
    }

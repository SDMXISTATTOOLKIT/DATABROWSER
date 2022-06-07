const importUncachedCSS = href => {
  let link = document.createElement("link");
  link.href = href + "?random=" + Math.floor(Math.random() * 16777215).toString(16);
  link.rel = "stylesheet";
  document.head.appendChild(link);
};

const importUncachedJS = src => {
  let script = document.createElement("script");
  script.src = src + "?random=" + Math.floor(Math.random() * 16777215).toString(16);
  document.head.appendChild(script);
};
export function initialize(lastItemIndicator, componentInstance) {
    const options = {
        root: findClosestScrollContainer(lastItemIndicator),
        rootMargin: '0px',
        threshold: 0,
    };

    const observer = new IntersectionObserver(async (entries) => {
        for (const entry of entries) {
            if (entry.isIntersecting) {
                await componentInstance.invokeMethodAsync("LoadMoreItems");
            }
        }
    }, options);

    observer.observe(lastItemIndicator);

    return {
        dispose: () => dispose(observer),
        onNewItems: () => {
            observer.unobserve(lastIndicator);
            observer.observe(lastIndicator);
        },
    };
}

function dispose(observer) {
    observer.disconnect();
}

function findClosestScrollContainer(element) {
    while (element) {
        const style = getComputedStyle(element);
        if (style.overflowY !== 'visible') {
            return element;
        }
        element = element.parentElement;
    }
    return null;
}
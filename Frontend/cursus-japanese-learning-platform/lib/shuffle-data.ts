export const shuffleData = <T>(data: Array<T>): Array<T> => {
    for (let index = data.length - 1; index > 0; index--) {
        const randomIndex = Math.floor(Math.random() * (index + 1));
        [data[index], data[randomIndex]] = [data[randomIndex], data[index]];
    }
    return data;
};